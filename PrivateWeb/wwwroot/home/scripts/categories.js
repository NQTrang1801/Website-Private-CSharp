import { products } from "../data/products.js";
import { cart, sumCostData, save } from "../data/cart-data.js";
// Khởi tạo Swiper
const swiper = new Swiper('.sliderbox', {
  loop: true,
  effect: 'fade',
  autoHeight: true,
  pagination: {
    el: '.swiper-pagination',
    clickable: true,
  },
});

document.addEventListener("DOMContentLoaded", function () {

  const path = window.location.pathname;

  const pathComponents = path.split('/').filter(component => component !== '');
  const [category, subcategory] = pathComponents;
  
  document.querySelector(".slider .title").innerHTML = decodeURIComponent(subcategory.toUpperCase());

  function renderIteamHTML(items, limit) {
    let html = '';
    let i = 0;
    items.forEach(item => {
      if (i < limit) {
        i++;
      } else {
        return html;
      }
      html += `
      <div class="item">
      <div class="dot-image">
          <a href="" class="product-permalink"></a>
          <div class="thumbnail">
              <img src="${item.image[0]}" alt="">
          </div>
          <div class="thumbnail hover">
              <img src="${item.image[1]}" alt="">
          </div>
          <div class="actions">
              <div><i class="ri-heart-line"></i></div>
              <div class="js-btn-add-cart" data-product-id="${item.productId}"><i class="ri-shopping-cart-line"></i></div>
              <a href="products/${subcategory}/${item.productId}"><i class="ri-eye-line"></i></a>
          </div>
          <div class="label"><span>-${item.sales * 100}%</span></div>
      </div>
      <div class="dot-info">
          <h2 class="dot-title"><a href="">${item.name}</a></h2>
          <div class="product-price">
              <span class="before">${item.price} VND</span>
              <span class="current">${item.price * (1 - item.sales)} VND</span>
          </div>
      </div>
   </div>
      `;

    });
    return html;
  }

  function renderProducts(container, typeName, typeIndex, maxShow, minShow) {
    let flag = false;
    const productsType = products.filter((item) => item.classify[0] == subcategory && item.classify[typeIndex] == typeName);
    if (maxShow <= productsType.length || productsType.length < minShow) {
      if (maxShow < productsType.length) {
        maxShow += minShow;
        flag = true;
      }
      container.innerHTML = '';
      container.innerHTML = renderIteamHTML(productsType, maxShow);
      addToCart(container);
    }
    return flag;
  }

  //Special Prices
  const divItemTop = document.querySelector('.special-prices .wrapper');
  let maxShowTop = 0;
  let minShowTop = 8;

  if (renderProducts(divItemTop, "special-prices", 2, maxShowTop, minShowTop)) {
    maxShowTop += minShowTop;
  }

  const btnSeeMoreTop = document.querySelector('.special-prices .see-more');
  btnSeeMoreTop.addEventListener('click', () => {
    if (renderProducts(divItemTop, "special-prices", 2, maxShowTop, minShowTop)) {
      maxShowTop += minShowTop;
    };
  });

  // Products Categories

  // Category-1
  const divItemC1 = document.querySelector('.type-1 .wrapper');
  let maxShowC1 = 0;
  let minShowC1 = 18;

  if (renderProducts(divItemC1, "coats-and-jackets", 4, maxShowC1, minShowC1)) {
    maxShowC1 += minShowC1;
  }

  const btnSeeMoreC1 = document.querySelector('.type-1 .see-more');
  btnSeeMoreC1.addEventListener('click', () => {
    if (renderProducts(divItemC1, "coats-and-jackets", 4, maxShowC1, minShowC1)) {
      maxShowC1 += minShowC1;
    };
  });

  // Category-2
  const divItemC2 = document.querySelector('.type-2 .wrapper');
  let maxShowC2 = 0;
  let minShowC2 = 10;

  if (renderProducts(divItemC2, "dresses-and-skirts", 4, maxShowC2, minShowC2)) {
    maxShowC2 += minShowC2;
  }

  const btnSeeMoreC2 = document.querySelector('.type-2 .see-more');
  btnSeeMoreC2.addEventListener('click', () => {
    if (renderProducts(divItemC2, "dresses-and-skirts", 4, maxShowC2, minShowC2)) {
      maxShowC2 += minShowC2;
    };
  });
  // Category-3
  const divItemC3 = document.querySelector('.type-3 .wrapper');
  let maxShowC3 = 0;
  let minShowC3 = 18;

  if (renderProducts(divItemC3, "pants-and-shorts", 4, maxShowC3, minShowC3)) {
    maxShowC3 += minShowC3;
  }

  const btnSeeMoreC3 = document.querySelector('.type-3 .see-more');
  btnSeeMoreC3.addEventListener('click', () => {
    if (renderProducts(divItemC3, "pants-and-shorts", 4, maxShowC3, minShowC3)) {
      maxShowC3 += minShowC3;
    };
  });

  // Category-4
  const divItemC4 = document.querySelector('.type-4 .wrapper');
  let maxShowC4 = 0;
  let minShowC4 = 10;

  if (renderProducts(divItemC4, "tops-and-shirts", 4, maxShowC4, minShowC4)) {
    maxShowC4 += minShowC4;
  }

  const btnSeeMoreC4 = document.querySelector('.type-4 .see-more');
  btnSeeMoreC4.addEventListener('click', () => {
    if (renderProducts(divItemC4, "tops-and-shirts", 4, maxShowC4, minShowC4)) {
      maxShowC4 += minShowC4;
    };
  });

  // Add To Cart
  function addToCart(DOM) {
    DOM.querySelectorAll('.js-btn-add-cart')
      .forEach((DomAddCart) => {
        DomAddCart.addEventListener('click', () => {
          console.log("click");
          const productId = DomAddCart.dataset.productId;
          const product = { ...products.find(item => item.productId === productId) };
          product.quantity = 1;
          product.vouchers = [];
          cart.push(product);
          save();
          renderDropDownCart();
        });
      });
  }

  function renderDropDownCart() {
    let htmlDropdownCart = ``;
    const divCartContent = document.querySelector('.js-cart-content');
    let sumQuantityCart = 0;

    cart.forEach(item => {
      sumQuantityCart += item.quantity;
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
                <p><span>${item.price * (1 - item.sales)}</span> VND</p>
              </div>
            </div>
            `;
    });
    divCartContent.innerHTML = htmlDropdownCart;
    const shoppingBag = document.querySelector('.ri-shopping-bag-line');
    shoppingBag.setAttribute('data-content', sumQuantityCart);
    document.querySelector('.cart-price').querySelector('p').nextElementSibling.querySelector('span').innerHTML = sumCostData();
    document.querySelector('.cart-checkout').querySelector('span').innerHTML = sumQuantityCart;
  };
});


document.getElementById('special-prices-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('special-prices');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});

document.getElementById('products-categories-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('products-categories');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});

document.getElementById('category-view-1-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('category-view-1');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});

document.getElementById('type-1-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('category-view-1');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});

document.getElementById('type-2-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('category-view-2');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});

document.getElementById('type-3-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('category-view-3');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});

document.getElementById('type-4-link').addEventListener('click', function (e) {
  e.preventDefault(); // Prevent default behavior of the link

  const section = document.getElementById('category-view-4');

  if (section) {
    section.scrollIntoView({
      behavior: 'smooth'
    });
  }
});
