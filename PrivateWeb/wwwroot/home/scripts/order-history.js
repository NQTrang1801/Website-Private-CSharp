let listItems = document.querySelectorAll(".time-lists li");

listItems.forEach((item) => {
  item.addEventListener('click', function() {
    listItems.forEach((li) => {
      li.style.fontWeight = 'normal';
      li.style.color = 'var(--secondary-light-color)'; 
    });

    this.style.fontWeight = 'bold';
    this.style.color = 'var(--primary-color)'; 
  });
});