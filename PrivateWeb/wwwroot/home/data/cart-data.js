export const cart = JSON.parse(localStorage.getItem("cart")) || [];

export function save() {
    localStorage.setItem("cart", JSON.stringify(cart));
}

// tổng chi phí
export function sumCostData() {
    const totalPrice = cart.reduce((cost, product) => {
        return cost + product.price * (1 - product.sales) * product.quantity;
    }, 0);
    return totalPrice;
}