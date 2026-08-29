// stockalert.js — StockAlert module
// Depends on: site.js (toggleAlertDropdown is defined there)
//
// Public API:
//   StockAlertService.initPage()       — call on StockAlert/Index page (table resolve)
//   StockAlertService.initBell()       — call on any page that shows the bell dropdown
//   StockAlertService.startAutoRefresh()  — call on dashboard / any persistent page

const StockAlertService = (() => {

    // ─── Shared helpers ──────────────────────────────────────────────────────

    /**
     * Read the ASP.NET Core antiforgery token from the page.
     * Works whether the token is in a hidden input or a meta tag.
     */
    function _getAntiforgeryToken() {
        const input = document.querySelector('input[name="__RequestVerificationToken"]');
        if (input) return input.value;
        const meta = document.querySelector('meta[name="RequestVerificationToken"]');
        if (meta) return meta.getAttribute('content');
        return null;
    }

    /**
     * POST to /StockAlert/Resolve/{id} with the antiforgery token.
     * Returns the parsed JSON result or null on network error.
     */
    async function _resolveRequest(alertId) {
        const token = _getAntiforgeryToken();
        try {
            const response = await fetch(`/StockAlert/Resolve/${alertId}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(token ? { 'RequestVerificationToken': token } : {})
                }
            });
            return await response.json();
        } catch (err) {
            console.error('StockAlertService: resolve request failed', err);
            return null;
        }
    }

    // ─── Full-page table (StockAlert/Index) ──────────────────────────────────

    /**
     * Handle "Resolve" button click in the full-page alerts table.
     * Removes the row on success; reloads page if table becomes empty.
     */
    async function resolveAlertPage(alertId, btn) {
        if (!confirm('Mark this alert as resolved?')) return;

        btn.disabled = true;
        const result = await _resolveRequest(alertId);

        if (result?.success) {
            const row = btn.closest('tr');
            if (row) {
                row.style.transition = 'opacity 0.25s';
                row.style.opacity = '0';
                setTimeout(() => {
                    row.remove();
                    // If no data rows remain, reload to show the "healthy" empty state
                    if (!document.querySelector('#alertsTbody tr')) {
                        location.reload();
                    }
                }, 260);
            }
        } else {
            btn.disabled = false;
            alert(result?.message || 'Error resolving alert. Please try again.');
        }
    }

    /**
     * Wire up all Resolve buttons in the #alertsTbody table.
     * Replaces inline onclick attributes with addEventListener.
     */
    function initPage() {
        // Support both the onclick="resolveAlertPage(...)" pattern (legacy)
        // AND the data-attribute pattern going forward.
        // The onclick calls come from the view, so we just expose the function globally too.
        window.resolveAlertPage = resolveAlertPage;
    }

    // ─── Bell dropdown (AlertNotification ViewComponent) ─────────────────────

    /**
     * Resolve an alert from the bell dropdown panel.
     * Animates the item out, updates the badge, shows empty state if needed.
     */
    async function resolveAlert(alertId, btn) {
        btn.disabled = true;

        const result = await _resolveRequest(alertId);

        if (result?.success) {
            const item = btn.closest('.alert-item');
            if (item) {
                item.style.transition = 'opacity 0.2s, transform 0.2s';
                item.style.opacity = '0';
                item.style.transform = 'translateX(20px)';
                setTimeout(() => {
                    item.remove();
                    _updateBadge();
                }, 210);
            }
        } else {
            btn.disabled = false;
            alert(result?.message || 'Failed to resolve alert.');
        }
    }

    /**
     * Recount visible .alert-item elements and update the badge + header.
     * Called after each dropdown resolve.
     */
    function _updateBadge() {
        const remaining = document.querySelectorAll('.alert-item').length;
        const badge     = document.querySelector('.alert-badge');
        const countEl   = document.querySelector('.alert-count');
        const menu      = document.getElementById('alertMenu');

        if (remaining === 0) {
            // Remove badge from bell
            if (badge) badge.remove();

            // Swap list + footer for empty state
            const list   = menu?.querySelector('.alert-list');
            const footer = menu?.querySelector('.alert-footer');
            if (list)   list.remove();
            if (footer) footer.remove();
            if (countEl) countEl.remove();

            if (menu && !menu.querySelector('.alert-empty')) {
                const emptyDiv = document.createElement('div');
                emptyDiv.className = 'alert-empty';
                emptyDiv.innerHTML = '<i class="bi bi-check-circle"></i><span>No active stock alerts</span>';
                menu.appendChild(emptyDiv);
            }
        } else {
            if (badge)   badge.textContent   = remaining;
            if (countEl) countEl.textContent = `${remaining} active`;
        }
    }

    /**
     * Initialise the bell dropdown on any page that renders the
     * AlertNotification ViewComponent (exposes resolveAlert globally).
     */
    function initBell() {
        window.resolveAlert = resolveAlert;
    }

    // ─── Auto-refresh badge polling ───────────────────────────────────────────

    /**
     * Poll /StockAlert/GetActiveAlerts every `intervalMs` milliseconds
     * and update only the badge counter on the bell.
     * Default interval: 60 seconds.
     */
    function startAutoRefresh(intervalMs = 60000) {
        setInterval(async () => {
            try {
                const res  = await fetch('/StockAlert/GetActiveAlerts');
                const data = await res.json();
                _syncBadgeCount(data.count);
            } catch (err) {
                console.warn('StockAlertService: auto-refresh failed', err);
            }
        }, intervalMs);
    }

    /**
     * Sync the badge number on the bell to a given count.
     * Creates or removes the badge element as needed.
     */
    function _syncBadgeCount(count) {
        const bell  = document.getElementById('alertBell');
        const badge = document.querySelector('.alert-badge');

        if (count > 0) {
            if (badge) {
                badge.textContent = count;
            } else if (bell) {
                const newBadge = document.createElement('span');
                newBadge.className   = 'alert-badge';
                newBadge.textContent = count;
                bell.appendChild(newBadge);
            }
        } else if (badge) {
            badge.remove();
        }
    }

    // ─── Public API ──────────────────────────────────────────────────────────
    return { initPage, initBell, startAutoRefresh };

})();
