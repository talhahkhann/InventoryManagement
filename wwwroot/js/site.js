// site.js — Reusable static utilities for InventoryManagement UI

// 1. Prevent HTML injection when inserting dynamic content
function escapeHtml(value) {
    if (value === null || value === undefined) return "";
    return String(value)
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}

// 2. Email Validation utility for form submissions
function registerEmailValidator(formSelector, emailInputName) {
    document.addEventListener('DOMContentLoaded', function () {
        const form = document.querySelector(formSelector);
        if (form) {
            form.addEventListener('submit', function (e) {
                // Clear previous errors
                document.querySelectorAll('.custom-error').forEach(el => el.remove());

                const email = document.querySelector(`[name="${emailInputName}"]`);
                if (email && email.value && !/^\S+@\S+\.\S+$/.test(email.value)) {
                    e.preventDefault();
                    
                    const error = document.createElement('div');
                    error.className = 'custom-error error-message';
                    error.textContent = 'Please enter a valid email address';
                    
                    email.parentNode.appendChild(error);
                    email.focus();
                }
            });
        }
    });
}

// 3. Stock Alert Notification Droplist Actions
function toggleAlertDropdown() {
    const menu = document.getElementById('alertMenu');
    if (menu) menu.classList.toggle('show');
}

// Register close handler for alerts dropdown
document.addEventListener('click', function(e) {
    const dropdown = document.querySelector('.alert-dropdown');
    if (dropdown && !dropdown.contains(e.target)) {
        const menu = document.getElementById('alertMenu');
        if (menu) menu.classList.remove('show');
    }
});

// 4. Generic Autocomplete search input helper
function initAutocomplete(inputElement, valueInputElement, itemsList, onSelectCallback) {
    let currentFocus;
    
    inputElement.addEventListener("input", function(e) {
        const val = this.value.trim();
        closeAllLists();
        if (!val) return;

        currentFocus = -1;
        const list = document.createElement("DIV");
        list.setAttribute("class", "autocomplete-items");
        this.parentNode.appendChild(list);

        let found = false;
        for (let i = 0; i < itemsList.length; i++) {
            if (itemsList[i].name.toLowerCase().includes(val.toLowerCase())) {
                found = true;
                const item = document.createElement("DIV");
                item.innerHTML = `<strong>${itemsList[i].name}</strong> - PKR ${parseFloat(itemsList[i].price).toFixed(2)}`;
                item.innerHTML += "<input type='hidden' value='" + itemsList[i].id + "'>";
                item.addEventListener("click", function(e) {
                    inputElement.value = itemsList[i].name;
                    valueInputElement.value = itemsList[i].id;
                    
                    if (onSelectCallback) {
                        onSelectCallback(itemsList[i]);
                    }
                    closeAllLists();
                });
                list.appendChild(item);
            }
        }

        if (!found) {
            const item = document.createElement("DIV");
            item.textContent = "No products found";
            item.style.color = "#999";
            list.appendChild(item);
        }
    });

    inputElement.addEventListener("keydown", function(e) {
        const list = this.parentNode.querySelector(".autocomplete-items");
        if (!list) return;
        let items = list.getElementsByTagName("div");
        if (e.keyCode == 40) { currentFocus++; addActive(items); }
        else if (e.keyCode == 38) { currentFocus--; addActive(items); }
        else if (e.keyCode == 13) {
            e.preventDefault();
            if (currentFocus > -1 && items[currentFocus]) {
                items[currentFocus].click();
            }
        }
    });

    function addActive(items) {
        if (!items) return;
        removeActive(items);
        if (currentFocus >= items.length) currentFocus = 0;
        if (currentFocus < 0) currentFocus = items.length - 1;
        items[currentFocus].classList.add("highlight");
    }

    function removeActive(items) {
        for (let i = 0; i < items.length; i++) {
            items[i].classList.remove("highlight");
        }
    }

    document.addEventListener("click", function (e) {
        if (e.target !== inputElement) closeAllLists();
    });

    function closeAllLists(elmnt) {
        const items = document.getElementsByClassName("autocomplete-items");
        for (let i = 0; i < items.length; i++) {
            if (elmnt != items[i] && elmnt != inputElement) {
                items[i].parentNode.removeChild(items[i]);
            }
        }
    }
}
