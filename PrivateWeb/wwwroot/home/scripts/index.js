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

});


