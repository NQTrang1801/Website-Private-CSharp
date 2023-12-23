
const swiper = new Swiper('.sliderbox', {
  loop: true,
  effect: 'fade',
  autoHeight: true,
  pagination: {
    el: '.swiper-pagination',
    clickable: true,
  },
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
