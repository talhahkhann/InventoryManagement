// invoice.js — Invoice module business logic and form interactive behaviors

let productsList = [];
let areasList = [];
let customersList = [];

async function fetchCustomerRate(customerId, productId) {
    if (!customerId || !productId) return null;
    try {
        const response = await fetch(`/Invoice/GetProductRate?customerId=${customerId}&productId=${productId}`);
        if (!response.ok) return null;
        return await response.json();
    } catch (err) {
        console.error('Rate lookup failed:', err);
        return null;
    }
}

function getCurrentCustomerId() {
    return document.getElementById('CustomerId').value;
}

async function applyRateToRow(row, productId) {
    const priceInput = row.querySelector('.price');
    const noteEl = row.querySelector('.price-note');
    const customerId = getCurrentCustomerId();

    if (!productId) return;

    const defaultProduct = productsList.find(p => p.id == productId);
    const defaultPrice = defaultProduct ? parseFloat(defaultProduct.price) : 0;

    if (!customerId) {
        priceInput.value = defaultPrice.toFixed(2);
        if (noteEl) { noteEl.textContent = ''; noteEl.classList.remove('custom'); }
        recalcRow(row);
        return;
    }

    const result = await fetchCustomerRate(customerId, productId);

    if (result && typeof result.rate === 'number') {
        priceInput.value = result.rate.toFixed(2);
        if (noteEl) {
            if (result.isCustomRate) {
                noteEl.textContent = 'Custom customer rate';
                noteEl.style.color = '#dd6b20';
            } else {
                noteEl.textContent = 'Default price';
                noteEl.style.color = '#718096';
            }
        }
    } else {
        priceInput.value = defaultPrice.toFixed(2);
        if (noteEl) { noteEl.textContent = ''; }
    }

    recalcRow(row);
}

function recalcRow(row) {
    const price = parseFloat(row.querySelector('.price').value) || 0;
    const qty = parseInt(row.querySelector('.quantity').value) || 0;
    const total = price * qty;
    row.querySelector('.total').value = total.toFixed(2);
    recalcTotal();
}

function recalcTotal() {
    let total = 0;
    document.querySelectorAll('.total').forEach(input => {
        total += parseFloat(input.value) || 0;
    });
    document.getElementById('grandTotal').textContent = total.toFixed(2);
}

function updateRowNumbers() {
    document.querySelectorAll('.invoiceRow').forEach((row, index) => {
        row.querySelector('.rowIndex').textContent = index + 1;
        const productId = row.querySelector('.productId');
        const qty = row.querySelector('.quantity');
        const price = row.querySelector('.price');
        const productNameHidden = row.querySelector('[name$=".ProductName"]');
        
        productId.name = `Items[${index}].ProductId`;
        qty.name = `Items[${index}].Quantity`;
        price.name = `Items[${index}].Price`;
        if (productNameHidden) productNameHidden.name = `Items[${index}].ProductName`;
    });
}

function addNewRow() {
    const firstRow = document.querySelector('.invoiceRow');
    if (!firstRow) return;
    
    const newRow = firstRow.cloneNode(true);
    const productNameInput = newRow.querySelector('.productName');
    const productIdInput = newRow.querySelector('.productId');
    const priceInput = newRow.querySelector('.price');
    const qtyInput = newRow.querySelector('.quantity');
    const totalInput = newRow.querySelector('.total');
    const noteEl = newRow.querySelector('.price-note');

    productNameInput.value = '';
    productIdInput.value = '';
    priceInput.value = '0.00';
    qtyInput.value = '1';
    totalInput.value = '0.00';
    if (noteEl) { noteEl.textContent = ''; }

    const existingList = newRow.querySelector('.autocomplete-items');
    if (existingList) existingList.remove();

    initRow(newRow);
    document.querySelector('#invoiceTable tbody').appendChild(newRow);
    updateRowNumbers();
    productNameInput.focus();
}

function initRow(row) {
    const input = row.querySelector('.productName');
    const productIdInput = row.querySelector('.productId');
    const priceInput = row.querySelector('.price');
    const quantityInput = row.querySelector('.quantity');

    initAutocomplete(input, productIdInput, productsList, async function(selectedProduct) {
        const productNameHidden = input.closest('tr').querySelector('[name$=".ProductName"]');
        if (productNameHidden) productNameHidden.value = selectedProduct.name;

        await applyRateToRow(row, selectedProduct.id);
    });
    quantityInput.addEventListener('input', () => recalcRow(row));
    priceInput.addEventListener('input', () => recalcRow(row));
}

function collectFormData() {
    const areaId = document.getElementById('AreaId').value;
    const customerId = document.getElementById('CustomerId').value;
    const customerName = document.getElementById('CustomerId').options[document.getElementById('CustomerId').selectedIndex]?.text || '';
    
    const items = [];
    document.querySelectorAll('.invoiceRow').forEach(row => {
        const productId = row.querySelector('.productId').value;
        if (productId) {
            items.push({
                productName: row.querySelector('.productName').value,
                price: parseFloat(row.querySelector('.price').value) || 0,
                quantity: parseInt(row.querySelector('.quantity').value) || 0,
                total: parseFloat(row.querySelector('.total').value) || 0
            });
        }
    });

    const grandTotal = parseFloat(document.getElementById('grandTotal').textContent) || 0;

    return { areaId, customerId, customerName, items, grandTotal };
}

function validateForm(data) {
    if (!data.customerId) {
        alert('Please select a customer.');
        return false;
    }
    if (data.items.length === 0) {
        alert('Please add at least one invoice item.');
        return false;
    }
    return true;
}

function generatePreviewHtml(data) {
    const today = new Date().toLocaleDateString('en-US', { 
        year: 'numeric', 
        month: 'long', 
        day: 'numeric' 
    });

    let itemsHtml = '';
    data.items.forEach((item, index) => {
        itemsHtml += `
            <tr>
                <td>${index + 1}</td>
                <td>${item.productName}</td>
                <td>PKR ${item.price.toFixed(2)}</td>
                <td>${item.quantity}</td>
                <td>PKR ${item.total.toFixed(2)}</td>
            </tr>
        `;
    });

    return `
        <div class="invoice-header">
            <h1>INVOICE</h1>
            <p>Invoice Date: ${today}</p>
        </div>
        <div class="invoice-meta">
            <div>
                <strong>From:</strong><br>
                Inventory Management System<br>
                Company Address Lane<br>
                City, State Country
            </div>
            <div>
                <strong>Billed To:</strong><br>
                ${data.customerName}<br>
                Customer ID: ${data.customerId}
            </div>
        </div>
        <table class="invoice-table">
            <thead>
                <tr>
                    <th>#</th>
                    <th>Product</th>
                    <th>Price</th>
                    <th>Qty</th>
                    <th>Total</th>
                </tr>
            </thead>
            <tbody>
                ${itemsHtml}
            </tbody>
        </table>
        <div class="invoice-total">
            Grand Total: <span>PKR ${data.grandTotal.toFixed(2)}</span>
        </div>
    `;
}

function showPreview() {
    const formData = collectFormData();
    if (!validateForm(formData)) return;

    const previewHtml = generatePreviewHtml(formData);
    document.getElementById('invoicePreviewContent').innerHTML = previewHtml;
    document.getElementById('previewModal').style.display = 'flex';
}

function closePreview() {
    document.getElementById('previewModal').style.display = 'none';
}

function printPreview() {
    const printContent = document.getElementById('invoicePreviewContent').innerHTML;
    const originalBody = document.body.innerHTML;
    
    document.body.innerHTML = `
        <div style="padding: 20px; max-width: 800px; margin: 0 auto;">
            ${printContent}
        </div>
    `;
    
    window.print();
    document.body.innerHTML = originalBody;
    location.reload();
}

function initInvoiceForm(products, areas, allCustomers) {
    productsList = products;
    areasList = areas;
    customersList = allCustomers;

    $('#AreaId').change(function () {
        const areaId = $(this).val();
        const customerDropdown = $('#CustomerId');
        customerDropdown.empty().append('<option value="">-- Select Customer --</option>');
        if (areaId) {
            const filtered = customersList.filter(c => c.areaId == areaId);
            filtered.forEach(customer => {
                customerDropdown.append(`<option value="${customer.id}">${customer.name}</option>`);
            });
        }
    });

    $('#CustomerId').change(function () {
        document.querySelectorAll('.invoiceRow').forEach(row => {
            const productId = row.querySelector('.productId').value;
            if (productId) {
                applyRateToRow(row, productId);
            }
        });
    });

    document.querySelectorAll('.invoiceRow').forEach(row => {
        initRow(row);
    });
    recalcTotal();

    document.getElementById('addRow').addEventListener('click', addNewRow);
    document.querySelector('#invoiceTable tbody').addEventListener('click', function(e) {
        if (e.target.closest('.removeRow')) {
            const rows = document.querySelectorAll('.invoiceRow');
            if (rows.length > 1) {
                e.target.closest('tr').remove();
                updateRowNumbers();
                recalcTotal();
            } else {
                alert('At least one invoice item is required.');
            }
        }
    });

    document.getElementById('previewBtn').addEventListener('click', showPreview);
    document.getElementById('closePreview').addEventListener('click', closePreview);
    document.getElementById('printBtn').addEventListener('click', printPreview);
    document.getElementById('saveFromPreview').addEventListener('click', () => {
        document.getElementById('invoiceForm').submit();
    });
}
