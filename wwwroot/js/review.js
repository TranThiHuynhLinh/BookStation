function updateClock() {
    var now = new Date();
    var dname = now.getDay(),
        mo = now.getMonth(),
        dnum = now.getDate(),
        yr = now.getFullYear(),
        hou = now.getHours(),
        min = now.getMinutes(),
        sec = now.getSeconds(),
        pe = "AM";

    if (hou >= 12) {
        pe = "PM";
    }
    if (hou == 0) {
        hou = 12;
    }
    if (hou > 12) {
        hou = hou - 12;
    }

    Number.prototype.pad = function (digits) {
        for (var n = this.toString(); n.length < digits; n = 0 + n);
        return n;
    }

    var months = ["January", "February", "March", "April", "May", "June", "July", "Augest", "September", "October", "November", "December"];
    var week = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
    var ids = ["dayname", "month", "daynum", "year", "hour", "minutes", "seconds", "period"];
    var values = [week[dname], months[mo], dnum.pad(2), yr, hou.pad(2), min.pad(2), sec.pad(2), pe];
    for (var i = 0; i < ids.length; i++)
        document.getElementById(ids[i]).firstChild.nodeValue = values[i];
}

function initClock() {
    updateClock();
    window.setInterval("updateClock()", 1);
}

const closeBtns = document.querySelectorAll('.close-btn');
closeBtns.forEach(function (btn) {
    btn.addEventListener("click", function () {
        document.querySelector('.write-model').style.display = "none";
        document.querySelector('body').style.overflow = "visible";
    });
});

date = new Date();
year = date.getFullYear();
month = date.getMonth() + 1;
day = date.getDate();
var currentDateElements = document.querySelectorAll('#current-date');
for (var i = 0; i < currentDateElements.length; i++) {
    currentDateElements[i].value = day + "-" + month + "-" + year;
}
const allStar = document.querySelectorAll('.write-model .rating .star')
const ratingValue = document.querySelector('.write-model .rating input')

allStar.forEach((item, idx) => {
    item.addEventListener('click', function () {
        let click = 0
        ratingValue.value = idx + 1

        allStar.forEach(i => {
            i.classList.replace('bxs-star', 'bx-star')
            i.classList.remove('active')
        })
        for (let i = 0; i < allStar.length; i++) {
            if (i <= idx) {
                allStar[i].classList.replace('bx-star', 'bxs-star')
                allStar[i].classList.add('active')
            } else {
                allStar[i].style.setProperty('--i', click)
                click++
            }
        }
    })
})

document.querySelector('.add-btn').addEventListener('click', function () {
    document.querySelector('#write-new-review').submit();
});

document.querySelector('.close-search-btn').addEventListener("click", function () {
    document.querySelector('.search-result-model').style.display = "none";
    document.querySelector('body').style.overflow = "visible";
    const searchParams = new URLSearchParams(window.location.search);
    searchParams.set('category_id', '');
    window.location.search = searchParams;
});

const searchParams = new URLSearchParams(window.location.search);
if (searchParams.get('category_id')) {
    document.querySelector('.search-result-model').style.display = "flex";
    document.querySelector('body').style.overflow = "hidden";
    var selectElement = document.querySelector('.category-select');
    var selectedValue = selectElement.value;
}