$(document).ready(function() {
  let currentCardIndex = 0;
  let cardSlides = $(".card-slide");
  let cardTimers = $(".cardstack-timer-run");
  let cardTimerIntervals = [];
  let isPaused = false;

  function moveToNextCard() {
    cardSlides.eq(currentCardIndex).hide();
    cardTimers.eq(currentCardIndex).css("width", "0%");
    currentCardIndex = (currentCardIndex + 1) % cardSlides.length;
    cardSlides.eq(currentCardIndex).show();
    startCardTimer(currentCardIndex);
  }

  // setInterval
  function startCardTimer(index) {
    let width = 0;
    cardTimerIntervals[index] = setInterval(function() {
      if (!isPaused) {
        width += 1;
        cardTimers.eq(index).css("width", width + "%");
        if (width >= 100) {
          clearInterval(cardTimerIntervals[index]);
          moveToNextCard();
        }
      }
    }, 30);
  }

  cardSlides.hide();
  cardSlides.eq(currentCardIndex).show();
  startCardTimer(currentCardIndex);

  // Right button
  $(".arrow-right").on("click", function() {
    clearInterval(cardTimerIntervals[currentCardIndex]);
    moveToNextCard();
  });

  // Left button
  $(".arrow-left").on("click", function() {
    cardSlides.eq(currentCardIndex).hide();
    cardTimers.eq(currentCardIndex).css("width", "0%");
    currentCardIndex = (currentCardIndex - 1 + cardSlides.length) % cardSlides.length;
    cardSlides.eq(currentCardIndex).show();
    startCardTimer(currentCardIndex);
  });

  // Pause button
  $(".pause").on("click", function() {
    isPaused = !isPaused;
    if (isPaused) {
      $(".pause i").removeClass("ri-pause-mini-line").addClass("ri-play-mini-line");
    } else {
      $(".pause i").removeClass("ri-play-mini-line").addClass("ri-pause-mini-line");
    }
  });
});