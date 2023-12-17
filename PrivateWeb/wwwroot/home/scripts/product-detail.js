import { cart, save } from "../data/cart-data.js";
import { products } from "../data/products.js";

document.addEventListener('DOMContentLoaded', () => {
  let color = "white";
  let size = "XS";

  const path = window.location.pathname;
  const pathComponents = path.split('/').filter(component => component !== '');
  let subcategory = '';
  let productId = '';
  let oldColor = '';
  let oldSize = '';
  let indexProduct = '';
  if (pathComponents.length == '3') {
    subcategory = pathComponents[1];
    productId = pathComponents[2];
  }
  else {
    productId = pathComponents[1];
    oldColor = pathComponents[2];
    oldSize = pathComponents[3];
    indexProduct = pathComponents[4];
  }

  const data = products.find(item => item.productId === productId);

  const domImageLeft = document.querySelector('.detail-image-left');
  const domImageRight = document.querySelector('.detail-image-right');
  const domSumary = document.querySelector('.product-detail-summary');

  let htmlImgLeft = `
    <div>
    <img src="${data.image[0]}" alt="">
  </div>
  <div>
    <img src="${data.image[1]}" alt="">
  </div>
  <div>
    <img src="${data.image[2]}" alt="">
  </div>
  <div>
    <img src="${data.image[3]}" alt="">
  </div>
    `;

  let htmlImgRight = `
        <img src="${data.image[0]}" alt=""></img>
    `;

  let htmlSumary = `
    <div class="detail-summary">
          <div class="summary-name">
            <p>${data.name}</p>
            <p class="detail-price">$${data.price * (1 - data.sales)}</p>
          </div>
          <div class="summary-color">
            <div>
              <p>Color</p>
              <p style="font-weight: 600;">${!indexProduct ? data.image_color[0].color : oldColor}</p>
            </div>
            <div class="summary-color-item">
              <div>
                <img src="${data.image_color[0].img}" data-content="0" alt="">
              </div>
              <div>
                <img src="${data.image_color[1].img}" data-content="1" alt="">
              </div>
              <div>
                <img src="${data.image_color[2].img}" data-content="2" alt="">
              </div>
            </div>
          </div>
          <div class="summary-size">
            <div>
              Size
              <i class="ri-arrow-down-s-line"></i>
            </div>
            <div>
              <p class="product-size-guide">Size Guide</p>
              <div class="product-size">
                <div>${data.sizes[0]}</div>
                <div>${data.sizes[1]}</div>
                <div>${data.sizes[2]}</div>
                <div>${data.sizes[3]}</div>
                <div>${data.sizes[4]}</div>
              </div>
            </div>
          </div>
          <div class="summary-button">
            <button class="js-btn-add-cart">Add to cart</button>
          </div>
          <div>
            <pre>${data.description}
              </pre>
          </div>
        </div>

        <div class="detail-reference">
          <div class="reference-title">
            <div>
              PRODUCTS DETAILS
              <i class="ri-arrow-down-s-line"></i>
            </div>
            <p>
              ${data.suggestion}
            </p>
          </div>
          <div class="reference-title">
            <div>
              CARE
              <i class="ri-arrow-down-s-line"></i>
            </div>
            <pre>
              ${data.care}
            </pre>
          </div>
        </div>
    `;

  domImageLeft.innerHTML = htmlImgLeft;
  domImageRight.innerHTML = htmlImgRight;
  domSumary.innerHTML = htmlSumary;

  const divLefts = domImageLeft.querySelectorAll('div');
  const divColors = document.querySelector('.summary-color-item').querySelectorAll('div');
  const divSizes = document.querySelector('.product-size').querySelectorAll('div');

  divLefts[0].classList.add("click-img-left");
  divLefts[0].style.borderBottom = "2px solid var(--primary-color)";

  if (!indexProduct) {
    divColors[0].style.borderBottom = "2px solid var(--primary-color)";
    divSizes[0].style.border = "3px solid var(--primary-color)";
  }
  else {
    const colorEx = products[0].image_color;
    colorEx.forEach((c, index) => {
      if (c.color === oldColor) {
        divColors[index].style.borderBottom = "2px solid var(--primary-color)";
      }
    });

    const sizeEx = products[0].sizes;
    sizeEx.forEach((s, index) => {
      if (s === oldSize) {
        divSizes[index].style.border = "3px solid var(--primary-color)";
      }
    });
  }


  divLefts.forEach((item) => {
    item.addEventListener('click', () => {
      domImageRight.querySelector('img').src = item.querySelector('img').src;
      item.classList.add("click-img-left");
      item.style.borderBottom = "2px solid var(--primary-color)";
      divLefts.forEach((div) => {
        if (div.innerHTML !== item.innerHTML) {
          div.classList.remove("click-img-left");
          div.style.borderBottom = "none";
        }
      })
    });
  });

  divColors.forEach(item => {
    item.addEventListener('click', () => {
      const content = item.querySelector("img").dataset.content;
      color = data.image_color[content].color;
      document.querySelector(".summary-color").querySelector("div").querySelector("p").nextElementSibling.textContent = data.image_color[content].color;
      item.style.borderBottom = "2px solid var(--primary-color)";
      divColors.forEach((div) => {
        if (div.innerHTML !== item.innerHTML) {
          div.style.borderBottom = "none";
        }
      })
    })
  });

  divSizes.forEach(item => {
    item.addEventListener('click', () => {
      size = item.innerHTML;
      item.style.border = "3px solid var(--primary-color)";
      divSizes.forEach((div) => {
        if (div.innerHTML !== item.innerHTML) {
          div.style.border = "1px solid var(--primary-color)";
        }
      })
    })
  });

  const btnBack = document.querySelector(".button-back");
  if (indexProduct) {
    document.querySelector(".js-btn-add-cart").innerHTML = "UPDATE";

    document.querySelector('.js-btn-add-cart').addEventListener('click', () => {
      cart[indexProduct].image_color[0].color = color;
      cart[indexProduct].sizes[0] = size;
      save();
      window.location.href = `products/${productId}/${color}/${size}/${indexProduct}`;
    });

    btnBack.addEventListener("click", () => {
      window.location.href = `cart`;
    })
  } else {
    btnBack.addEventListener("click", () => {
      window.location.href = `/categories/${subcategory}`;
    })

    document.querySelector('.js-btn-add-cart').addEventListener('click', () => {
      const product = { ...products.find(item => item.productId === productId) };
      product.image_color[0].color = color;
      product.sizes[0] = size;
      product.quantity = 1;
      product.vouchers = [];
      cart.push(product);
      save();
      location.reload();
    });
  }


});

