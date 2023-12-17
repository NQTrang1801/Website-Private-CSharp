import { products } from "../data/products.js";
import { cart } from "../data/cart-data.js";
// Scroll header
$(document).ready(function () {
  const headerElement = $(".private-header");
  const searchContainer = $(".js-search-navbar");
  const cartContainer = $(".js-cart-navbar");

  $(document).scroll(function () {
    if (window.scrollY > 0) {
      headerElement.addClass("scrolled");
      searchContainer.addClass("scrolled");
      cartContainer.addClass("scrolled");
    } else {
      headerElement.removeClass("scrolled");
      searchContainer.removeClass("scrolled");
      cartContainer.removeClass("scrolled");
    }
  });

    $('.dropdown-toggle').dropdown();

  // Search container
  $(".header-search").click(function () {
    $(".js-search-navbar").slideToggle("slow");
    $(".private-header").addClass("hovered");
    $(document).on("click", function (event) {
      const elementBox = $(".js-search-navbar");
      const elementIcon = $(".header-search");
      if (!elementBox.is(event.target) && elementBox.has(event.target).length === 0
        && !elementIcon.is(event.target) && elementIcon.has(event.target).length === 0) {
        $(".js-search-navbar").css({ "display": "none" });
      }
    })
  });

  // Menu container
  $(".js-menu-btn").click(function () {
    $('.js-menu-navbar').css({ "left": "0px" });
    $("#overlay").css({ "display": "block" });
    $("body").css({ "overflow": "hidden" });
  });

  $(".nav-close").click(function () {
    closeMenuBar();
  });

  $("#overlay").click(function (event) {
    closeMenuBar();
  });

  function closeMenuBar() {
    $(".js-menu-navbar").css({ "left": "-500px" });
    $("#overlay").css({ "display": "none" });
    $("body").css({ "overflow": "scroll" });
  }

  // render dropdown cart
  let htmlDropdownCart = ``;
  const divCartContent = document.querySelector('.js-cart-content');
  let sumQuantityCart = 0;
  let sumCost = 0;
  cart.forEach(item => {
    sumQuantityCart += item.quantity;
    sumCost += item.quantity * item.price*(1-item.sales);
    htmlDropdownCart += `
                <div class="cart-box">
                  <div class="cart-image">
                    <img src="${item.image[0]}">
                  </div>
                  <div class="cart-info">
                    <p>${item.name}</p>
                    <p>COLOR: <span>${item.image_color[0].color}</span></p>
                    <p>SIZE: <span>${item.sizes[0]}</span></p>
                    <p>QTY: <span>${item.quantity}</span></p>
                    <p>$<span>${item.price*(1-item.sales)}</span></p>
                  </div>
                </div>
  `
  });
  divCartContent.innerHTML = htmlDropdownCart;
  const shoppingBag = document.querySelector('.ri-shopping-bag-line');
  shoppingBag.setAttribute('data-content', sumQuantityCart);
  document.querySelector('.cart-price').querySelector('p').nextElementSibling.querySelector('span').innerHTML = sumCost;
  document.querySelector('.cart-checkout').querySelector('span').innerHTML = sumQuantityCart;

});


