// customer.js — Customer module interactions and table rendering

function initCustomerAreaFilter(getCustomersUrl) {
    const areaDropdown = document.getElementById("areaDropdown");
    const customerTableBody = document.getElementById("customerTableBody");
    const loading = document.getElementById("loading");

    if (!areaDropdown || !customerTableBody) return;

    areaDropdown.addEventListener("change", async function () {
        const areaId = this.value;

        // All areas selected
        if (!areaId) {
            window.location.reload();
            return;
        }

        if (loading) loading.classList.add("show");

        try {
            const response = await fetch(`${getCustomersUrl}?areaId=${encodeURIComponent(areaId)}`);

            if (!response.ok) {
                throw new Error("Failed to load customers.");
            }

            const customers = await response.json();

            // Clear current table
            customerTableBody.innerHTML = "";

            if (customers.length === 0) {
                customerTableBody.innerHTML = `
                    <tr>
                        <td colspan="6" style="text-align:center; padding:40px; color:#718096;">
                            <i class="bi bi-people" style="font-size:2rem; display:block; margin-bottom:10px;"></i>
                            No customers found in this area.
                        </td>
                    </tr>
                `;
                return;
            }

            customers.forEach(customer => {
                const row = document.createElement("tr");

                row.innerHTML = `
                    <td data-label="Name" class="name-cell">
                        ${escapeHtml(customer.name)}
                    </td>
                    <td data-label="Email" class="email-cell">
                        ${escapeHtml(customer.email)}
                    </td>
                    <td data-label="Phone" class="phone-cell">
                        ${escapeHtml(customer.phoneNumber)}
                    </td>
                    <td data-label="City" class="location-cell">
                        ${escapeHtml(customer.city)}
                    </td>
                    <td data-label="Country" class="location-cell">
                        ${escapeHtml(customer.country)}
                    </td>
                    <td data-label="Actions" style="text-align:center;">
                        <div class="action-buttons">
                            <a href="/Customer/Edit/${customer.id}" class="btn btn-warning">
                                <i class="bi bi-pencil"></i>
                            </a>
                            <form action="/Customer/Delete/${customer.id}" method="post" style="display:inline;">
                                <button type="submit" class="btn btn-danger" onclick="return confirm('Are you sure you want to delete this customer?');">
                                    <i class="bi bi-trash"></i>
                                </button>
                            </form>
                        </div>
                    </td>
                `;

                customerTableBody.appendChild(row);
            });
        }
        catch (error) {
            console.error(error);
            alert("Unable to load customers for this area.");
        }
        finally {
            if (loading) loading.classList.remove("show");
        }
    });
}
