const ADMIN_SECRET_PASS = "123456";

function checkPasswordAndSubmit(formId) {
    Swal.fire({
        title: 'Xác nhận bảo mật',
        text: 'Vui lòng nhập mật khẩu admin',
        input: 'password',
        inputPlaceholder: 'Nhập mật khẩu...',
        inputAttributes: {
            autocapitalize: 'off',
            autocorrect: 'off'
        },
        showCancelButton: true,
        confirmButtonText: 'Xác nhận',
        cancelButtonText: 'Hủy',
        confirmButtonColor: '#0d6efd',
        cancelButtonColor: '#6c757d',
        showLoaderOnConfirm: true,

        preConfirm: (password) => {
            if (!password) {
                Swal.showValidationMessage('Vui lòng nhập mật khẩu');
                return false;
            }

            if (password !== ADMIN_SECRET_PASS) {
                Swal.showValidationMessage('Mật khẩu không đúng');
                return false;
            }

            return true;
        }
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById(formId).submit();
        }
    });
}
