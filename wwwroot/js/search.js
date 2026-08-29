// search.js — TableSearchService
// Reusable client-side table search/filter. No dependencies.
//
// Usage:
//   TableSearchService.init({
//     inputId:        'tableSearch',      // id of the <input>
//     clearBtnId:     'tableSearchClear', // id of the clear × button
//     tbodyId:        'searchableTbody',  // id of the <tbody> to filter
//     emptyStateId:   'searchEmptyState', // id of the no-results row (optional)
//     columnIndexes:  [0, 1, 2],          // which <td> indexes to search (omit = all)
//   });
//
// The service can be re-run on demand (e.g. after AJAX row replacements):
//   TableSearchService.refresh('tableSearch');

const TableSearchService = (() => {

    // Registry of active search instances keyed by inputId
    const _instances = {};

    /**
     * Normalise text — lowercase, collapse whitespace.
     */
    function _normalise(text) {
        return (text || '').toLowerCase().replace(/\s+/g, ' ').trim();
    }

    /**
     * Wrap matching text in a <mark> highlight span.
     * Works on a single td's textContent only (does not alter HTML structure).
     */
    function _highlight(td, term) {
        const original = td.getAttribute('data-original-text');
        if (original === null) return; // safety guard
        if (!term) {
            td.textContent = original;
            return;
        }
        const idx = _normalise(original).indexOf(term);
        if (idx === -1) {
            td.textContent = original;
            return;
        }
        const before = original.slice(0, idx);
        const match  = original.slice(idx, idx + term.length);
        const after  = original.slice(idx + term.length);
        td.innerHTML = '';
        if (before) td.appendChild(document.createTextNode(before));
        const mark = document.createElement('mark');
        mark.className = 'search-highlight';
        mark.textContent = match;
        td.appendChild(mark);
        if (after) td.appendChild(document.createTextNode(after));
    }

    /**
     * Cache original text content of searchable cells so we can restore
     * them cleanly after highlight removal.
     */
    function _cacheOriginalText(tbody, columnIndexes) {
        Array.from(tbody.rows).forEach(row => {
            const cells = Array.from(row.cells);
            cells.forEach((td, idx) => {
                if (columnIndexes === null || columnIndexes.includes(idx)) {
                    if (td.getAttribute('data-original-text') === null) {
                        td.setAttribute('data-original-text', td.textContent);
                    }
                }
            });
        });
    }

    /**
     * Core filter function — run on every keyup.
     */
    function _filter(config) {
        const input   = document.getElementById(config.inputId);
        const tbody   = document.getElementById(config.tbodyId);
        const emptyEl = config.emptyStateId
            ? document.getElementById(config.emptyStateId)
            : null;
        const clearBtn = config.clearBtnId
            ? document.getElementById(config.clearBtnId)
            : null;

        if (!input || !tbody) return;

        const term     = _normalise(input.value);
        const colIdxs  = config.columnIndexes || null; // null = search all columns
        let   visible  = 0;

        // Re-cache in case rows were replaced by AJAX
        _cacheOriginalText(tbody, colIdxs);

        Array.from(tbody.rows).forEach(row => {
            const cells   = Array.from(row.cells);
            const targets = colIdxs
                ? cells.filter((_, i) => colIdxs.includes(i))
                : cells;

            const rowText = _normalise(targets.map(td => td.textContent).join(' '));
            const matches = !term || rowText.includes(term);

            row.style.display = matches ? '' : 'none';

            // Highlight in searchable cells
            targets.forEach(td => {
                if (td.getAttribute('data-original-text') !== null) {
                    _highlight(td, term);
                }
            });

            if (matches) visible++;
        });

        // Show / hide "no results" row
        if (emptyEl) {
            emptyEl.style.display = visible === 0 ? '' : 'none';
        } else {
            // Auto-inject a no-results row if none was provided
            let noResults = tbody.querySelector('.search-no-results');
            if (visible === 0 && !noResults) {
                const colspan = tbody.rows[0] ? tbody.rows[0].cells.length : 10;
                noResults = document.createElement('tr');
                noResults.className = 'search-no-results';
                noResults.innerHTML = `
                    <td colspan="${colspan}" class="search-no-results-cell">
                        <i class="bi bi-search"></i>
                        No results for <strong>${_escapeHtml(input.value)}</strong>
                    </td>`;
                tbody.appendChild(noResults);
            } else if (visible > 0 && noResults) {
                noResults.remove();
            }
        }

        // Toggle clear button visibility
        if (clearBtn) {
            clearBtn.style.display = term ? 'flex' : 'none';
        }

        // Persist last term on instance
        config._lastTerm = term;
    }

    function _escapeHtml(v) {
        return String(v)
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;');
    }

    /**
     * Initialise a search bar against a tbody.
     */
    function init(config) {
        const input    = document.getElementById(config.inputId);
        const clearBtn = config.clearBtnId
            ? document.getElementById(config.clearBtnId)
            : null;

        if (!input) return;

        // Store config for refresh calls
        _instances[config.inputId] = config;

        // Cache original text on init
        const tbody = document.getElementById(config.tbodyId);
        if (tbody) _cacheOriginalText(tbody, config.columnIndexes || null);

        // Live filter on keyup / input
        input.addEventListener('input', () => _filter(config));

        // Clear button
        if (clearBtn) {
            clearBtn.style.display = 'none';
            clearBtn.addEventListener('click', () => {
                input.value = '';
                _filter(config);
                input.focus();
            });
        }
    }

    /**
     * Re-run the filter for a given inputId.
     * Call this after rows are replaced by AJAX so the current search term
     * is immediately applied to the new rows.
     */
    function refresh(inputId) {
        const config = _instances[inputId];
        if (config) _filter(config);
    }

    return { init, refresh };

})();
