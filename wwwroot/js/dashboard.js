// dashboard.js — Home/Dashboard module
// Depends on: stockalert.js (startAlertBadgeAutoRefresh)

/**
 * Initialises the dashboard page:
 *  - Starts the stock-alert badge auto-refresh polling loop (60 s interval).
 */
document.addEventListener('DOMContentLoaded', function () {
    startAlertBadgeAutoRefresh();
});
