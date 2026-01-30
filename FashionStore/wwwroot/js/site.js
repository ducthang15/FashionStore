window.addEventListener("scroll", function () {
    const nav = document.querySelector(".navbar");
    if (window.scrollY > 80) {
        nav.classList.add("scrolled");
    } else {
        nav.classList.remove("scrolled");
    }
});


//TYNE
tinymce.init({
    selector: '#Content',
    height: 550,

    menubar: true,

    plugins: [
        'advlist', 'autolink', 'lists', 'link', 'image', 'media',
        'charmap', 'preview', 'anchor', 'searchreplace',
        'visualblocks', 'code', 'fullscreen',
        'insertdatetime', 'table', 'help', 'wordcount',
        'emoticons'
    ],

    toolbar:
        'undo redo | blocks fontfamily fontsize | ' +
        'bold italic underline strikethrough | forecolor backcolor | ' +
        'alignleft aligncenter alignright alignjustify | ' +
        'bullist numlist outdent indent | ' +
        'link image media table emoticons | ' +
        'removeformat code fullscreen preview',


    fontsize_formats:
        '10px 12px 14px 16px 18px 20px 24px 28px 32px 36px',

    fontfamily_formats:
        'Arial=arial,helvetica,sans-serif;' +
        'Times New Roman=times new roman,times;' +
        'Roboto=roboto,sans-serif;' +
        'Tahoma=tahoma,arial;' +
        'Courier New=courier new,courier',
    images_upload_url: '/Admin/NewsAdmin/UploadImage',
    automatic_uploads: true,

    branding: false,
    language: 'vi'
});
