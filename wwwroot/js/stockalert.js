// stockalert.js — StockAlert module API integrations

// 1. Resolve alert inside the Stock Alerts main table view
async function resolveAlertPage(alertId, btn) {
    if (!confirm('Mark this alert as resolved?')) return;
    try {
        const response = await fetch(`/StockAlert/Resolve/${alertId}`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });
        const result = await response.json();
        if (result.success) {
            btn.closest('tr').remove();
            if (document.querySelectorAll('tbody tr').length === 0) {
                location.reload();
            }
        }
    } catch (e) { 
        alert('Error resolving alert.'); 
    }
}

// 2. Resolve stock alert dropdown item (layout bell)
async function resolveAlert(alertId, btn) {
    try {
        const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
        const response = await fetch(`/StockAlert/Resolve/${alertId}`, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token,
                'Content-Type': 'application/json'
            }
        });
        
        const result = await response.json();
        if (result.success) {
            const item = btn.closest('.alert-item');
            if (item) {
                item.style.opacity = '0';
                item.style.transform = 'translateX(20px)';
                setTimeout(() => {
                    item.remove();
                    updateAlertBadge();
                }, 200);
            }
        } else {
            alert('Failed to resolve alert');
        }
    } catch (err) {
        console.error(err);
        alert('Error resolving alert');
    }
}

function updateAlertBadge() {
    const remaining = document.querySelectorAll('.alert-item').length;
    const badge = document.querySelector('.alert-badge');
    const menu = document.getElementById('alertMenu');
    
    if (remaining === 0) {
        if (badge) badge.remove();
        const list = menu?.querySelector('.alert-list');
        const footer = menu?.querySelector('.alert-footer');
        if (list) list.remove();
        if (footer) footer.remove();
        
        if (menu && !menu.querySelector('.alert-list')) {
            const emptyDiv = document.createElement('div');
            emptyDiv.className = 'alert-empty';
            emptyDiv.innerHTML = '<i class="bi bi-check-circle"></i><span>No active stock alerts</span>';
            menu.appendChild(emptyDiv);
        }
    } else if (badge) {
        badge.textContent = remaining;
    }
}

function startAlertBadgeAutoRefresh() {
    setInterval(async () => {
        try {
            const res = await fetch('/StockAlert/GetActiveAlerts');
            const data = await res.json();
            const badge = document.querySelector('.alert-badge');
            const bell = document.getElementById('alertBell');
            
            if (data.count > 0) {
                if (badge) {
                    badge.textContent = data.count;
                } else if (bell) {
                    const newBadge = document.createElement('span');
                    newBadge.className = 'alert-badge';
                    newBadge.textContent = data.count;
                    bell.appendChild(newBadge);
                }
            } else if (badge) {
                badge.remove();
            }
        } catch (e) { 
            console.error('Auto-refresh failed:', e); 
        }
    }, 60000);
}
