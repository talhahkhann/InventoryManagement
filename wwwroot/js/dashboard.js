// dashboard.js — Home/Dashboard page init
// Depends on: stockalert.js (StockAlertService)

document.addEventListener('DOMContentLoaded', function () {
    // Wire up bell dropdown resolve buttons
    StockAlertService.initBell();

    // Start polling badge count every 60 seconds
    StockAlertService.startAutoRefresh();
});
