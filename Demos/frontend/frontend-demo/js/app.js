// const catalogItems = [
//     { sku: "BK-101", name: "Clean Code",                price: 29.99, currentStock: 12 },
//     { sku: "BK-102", name: "The Pragmatic Programmer",  price: 34.99, currentStock: 7 },
//     { sku: "BK-103", name: "Design Patterns",           price: 44.99, currentStock: 3 },
//     { sku: "BK-104", name: "Refactoring",               price: 39.99, currentStock: 0 },
// ];
const catalogItems = [];


function renderCards (items) {
    const container = document.querySelector("#catalog-cards"); // GetElementById works the same way

    if (items.length === 0) {
        container.innerHTML = `<p class="hint"> nothing matches</p>`
        return;
    }
    container.innerHTML = items.map(item => `
        <article class="card" data-sku="${item.sku}">
            <h3>${item.name}</h3>
            <dl>
                <dt>SKU</dt><dd>${item.sku}</dd>
                <dt>Price</dt><dd>${item.price.toFixed(2)}</dd>
                <dt>In Stock</dt><dd>${item.currentStock}</dd>
            </dl>
            <button class="price-btn" data-sku="${item.sku}" OnClick=supplierPriceShow("${item.sku}")>Supplier price</button>
            <p class="supplier-price"></p>
        </article>
        `
    ).join("");
}

function supplierPriceShow(sku) {
    console.log("works "+sku);
}

document.querySelector("#search").addEventListener("input", (e) => {
    const search = e.target.value.trim().toLowerCase();
    renderCards(catalogItems.filter(item => item.name.toLowerCase().includes(search) || item.sku.toLowerCase().includes(search)));
});

async function loadCatalog(){
    const API = "http://localhost:5224/api"
    const container = document.getElementById("catalog-cards");

    const loading = document.createElement("p");
    loading.className = "hint";
    loading.textContent = "loading...";

    container.innerHTML = "";
    container.appendChild(loading);

    try {
        const response = await fetch(API+"/Inventory");
        if (!response.ok) {
            container.innerHTML = "API said" + response.status;
        }
        const data = await response.json();
        renderCards(data);
    } catch (err) {
        console.log(err);
        container.innerHTML = "Something went wrong";
    }
}

document.addEventListener("DOMContentLoaded", async () => await loadCatalog());
