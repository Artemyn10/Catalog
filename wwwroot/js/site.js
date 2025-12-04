document.addEventListener("DOMContentLoaded", () => {

    const burger = document.querySelector('.hamburger');
    const mobileMenu = document.querySelector('.mobile-menu');

    if (!burger || !mobileMenu) {
        console.log("Элементы меню не найдены");
        return;
    }

    burger.addEventListener('click', () => {
        mobileMenu.classList.toggle('active');
        console.log("Меню переключено");
    });

});
