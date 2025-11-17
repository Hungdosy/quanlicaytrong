// ====== CONFIG ======
const API_REGISTER = "https://localhost:7120/api/auth/register"; // hoặc "/api/auth/register" nếu cùng origin
const DEFAULT_ROLE_ID = 2;   // User
const LOGIN_URL = "/Auth/Login"; // Trang chuyển hướng sau khi đăng ký xong

// ====== HELPERS ======
function showErr(text) {
    $("#msg").removeClass("hidden text-green-600").addClass("text-red-600").text(text);
}
function showOk(text) {
    $("#ok").removeClass("hidden").text(text);
}

// ====== MAIN ======
$(function () {
    // Toggle hiển/ẩn mật khẩu
    $("#togglePwd").on("click", function () {
        const ip = $("#password");
        ip.attr("type", ip.attr("type") === "password" ? "text" : "password");
        $(this).text(ip.attr("type") === "password" ? "Hiện" : "Ẩn");
    });
    $("#togglePwd2").on("click", function () {
        const ip = $("#confirmPassword");
        ip.attr("type", ip.attr("type") === "password" ? "text" : "password");
        $(this).text(ip.attr("type") === "password" ? "Hiện" : "Ẩn");
    });

    // Enter để submit
    $("#registerForm input").on("keydown", function (e) {
        if (e.key === "Enter") $("#btnRegister").click();
    });

    $("#btnRegister").on("click", function () {
        $("#msg").addClass("hidden").text("");
        $("#ok").addClass("hidden").text("");

        const fullname = $("#fullname").val().trim();
        const email = $("#email").val().trim();
        const phone = $("#phone").val().trim();
        const password = $("#password").val();
        const confirm = $("#confirmPassword").val();

        // FE check nhẹ — validate sâu để API xử lý
        if (!fullname || !email || !phone || !password || !confirm) {
            return showErr("Vui lòng điền đủ tất cả các trường.");
        }
        if (password.length < 6) {
            return showErr("Mật khẩu tối thiểu 6 ký tự.");
        }
        if (password !== confirm) {
            return showErr("Mật khẩu nhập lại không khớp.");
        }

        const btn = $(this);
        btn.prop("disabled", true).text("Đang tạo tài khoản...");

        $.ajax({
            url: API_REGISTER,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: JSON.stringify({
                fullname: fullname,
                email: email,
                phone: phone,
                password: password,
                roleId: DEFAULT_ROLE_ID
            }),
            success: function (res) {
                showOk(res?.message || "Đăng ký thành công!");
                setTimeout(() => window.location.href = LOGIN_URL, 800);
            },
            error: function (xhr) {
                let msg = "";
                try {
                    const data = xhr.responseJSON || JSON.parse(xhr.responseText || "{}");
                    if (xhr.status === 409) {
                        msg = data?.message || "Email đã tồn tại.";
                    } else if (xhr.status === 400 && data?.errors) {
                        msg = Object.values(data.errors).flat().join("\n");
                    } else if (data?.message) {
                        msg = data.message;
                    } else if (data?.detail) {
                        msg = data.detail;
                    } else if (typeof data === "string" && data.trim() !== "") {
                        msg = data;
                    }
                } catch { /* ignore parse error */ }

                if (!msg) msg = `Đăng ký thất bại. Vui lòng thử lại${xhr.status ? " (HTTP " + xhr.status + ")" : ""}.`;
                showErr(msg);
            },
            complete: function () {
                btn.prop("disabled", false).text("Tạo tài khoản");
            }
        });
    });
});
