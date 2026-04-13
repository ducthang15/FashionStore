window.addEventListener("scroll", function () {
    const nav = document.querySelector(".navbar");
    if (window.scrollY > 80) {
        nav.classList.add("scrolled");
    } else {
        nav.classList.remove("scrolled");
    }
});

const btn = document.getElementById("backToTop");

if (btn) {
    document.addEventListener("scroll", () => {
        const scroll = document.documentElement.scrollTop;

        btn.style.display = scroll > 300 ? "block" : "none";
    });

    btn.addEventListener("click", (e) => {
        e.preventDefault();
        document.documentElement.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    });
}

$('#checkAll').on('change', function () {
    $('input[name="ids"]').prop('checked', $(this).prop('checked'));
});
$(document).on('click', '#btnDeleteSelect', function (e) {

    e.preventDefault();

    let checked = $('input[name="ids"]:checked').length;

    if (checked === 0) {
        Swal.fire({
            icon: 'warning',
            title: 'Chưa chọn sản phẩm',
            text: 'Vui lòng chọn ít nhất 1 sản phẩm!'
        });
        return;
    }

    Swal.fire({
        title: 'Xác nhận xoá?',
        text: `Bạn sắp xoá ${checked} sản phẩm!`,
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xoá ngay',
        cancelButtonText: 'Huỷ',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            $(this).closest('form').trigger('submit');
        }
    });
});
$(document).on('click', '#btnDeleteAll', function () {

    Swal.fire({
        title: 'Xoá toàn bộ?',
        text: 'Hành động này không thể hoàn tác!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Xoá tất cả',
        cancelButtonText: 'Huỷ',
        confirmButtonColor: '#d33'
    }).then((result) => {
        if (result.isConfirmed) {
            window.location.href = "/Admin/ProductAdmin/DeleteAll";
        }
    });
});
$(function () {
    const $mainImg = $("#mainImage");
    const $modalImg = $("#modalImage");
    const $modalEle = $('#imageModal');
    let scale = 1;
    $mainImg.on("click", function () {
        const currentSrc = $(this).attr("src");

        $modalImg.attr("src", currentSrc).css({
            "transform": "scale(1)",
            "transform-origin": "center center"
        });

        scale = 1;
        $modalEle.modal('show');
    });
    $modalImg.on("wheel", function (e) {
        e.preventDefault();
        const oe = e.originalEvent;
        const rect = this.getBoundingClientRect();
        const offsetX = oe.clientX - rect.left;
        const offsetY = oe.clientY - rect.top;
        const x = (offsetX / rect.width) * 100;
        const y = (offsetY / rect.height) * 100;
        scale += oe.deltaY < 0 ? 0.25 : -0.25;
        scale = Math.max(1, Math.min(scale, 5));

        $(this).css({
            "transform-origin": `${x}% ${y}%`,
            "transform": `scale(${scale})`,
            "cursor": scale > 1 ? "zoom-out" : "zoom-in"
        });
    });
    $modalEle.on('hidden.bs.modal', function () {
        scale = 1;
        $modalImg.css("transform", "scale(1)");
    });
});

