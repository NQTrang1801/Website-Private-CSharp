document.addEventListener("DOMContentLoaded", () => {
    const path = window.location.pathname;
    const pathComponents = path.split('/').filter(component => component !== '');
    const subtotal = pathComponents[1];
    const dataElements = document.querySelector(".inf-order").querySelectorAll("span");
    dataElements[0].innerHTML = subtotal;
    dataElements[1].innerHTML = 2;
    dataElements[2].innerHTML = Number(subtotal) + 2;
});