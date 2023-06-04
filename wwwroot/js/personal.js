const allBookImg = document.querySelectorAll('.book-img')
const allRemoveBtn = document.querySelectorAll('.remove-btn')

allBookImg.forEach((item, idx) => {
    item.addEventListener('mouseout', function () {
        allRemoveBtn[idx].classList.remove('active-btn');
    });

    item.addEventListener('mouseover', function () {
        allRemoveBtn[idx].classList.add('active-btn');
    });
});

allRemoveBtn.forEach((item, idx) => {
    item.addEventListener('mouseout', function () {
        item.classList.remove('active-btn');
        allBookImg[idx].classList.remove('img-blur')
    });

    item.addEventListener('mouseover', function () {
        item.classList.add('active-btn');
        allBookImg[idx].classList.add('img-blur')
    });
});

document.querySelector(".user-img").addEventListener('click', function () {
    window.location.href = '/Personal/UploadAvatar';
});

