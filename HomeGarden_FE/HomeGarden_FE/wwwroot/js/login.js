const API_LOGIN = "https://localhost:7120/api/auth/login"; // hoặc "/api/auth/login" nếu cùng origin

$("#togglePwd").on("click", function () {
    const pwd = $("#password");
    pwd.attr("type", pwd.attr("type") === "password" ? "text" : "password");
    $(this).text(pwd.attr("type") === "password" ? "Hiện" : "Ẩn");
});

$("#btnLogin").on("click", function () {
    $("#msg").addClass("hidden").text("");
    const email = $("#email").val().trim();
    const password = $("#password").val();
    const remember = $("#remember").is(":checked");

    if (!email || !password) {
        $("#msg").removeClass("hidden").text("Vui lòng nhập đầy đủ Email và Mật khẩu.");
        return;
    }

    $(this).prop("disabled", true).text("Đang đăng nhập...");

    $.ajax({
        url: API_LOGIN,
        type: "POST",
        contentType: "application/json",
        data: JSON.stringify({ email, password }),
        success: function (res) {
            // ✅ lấy data chuẩn nếu API có bọc lớp data
            const info = res.data ?? res;

            const payload = {
                token: info.token,
                userId: info.userId,
                fullname: info.fullname,
                role: info.role,
                ts: Date.now()
            };

            // Lưu token
            if (remember) {
                localStorage.setItem("hg_token", info.token);
                localStorage.setItem("hg_user", JSON.stringify(payload));
            } else {
                sessionStorage.setItem("hg_token", info.token);
                sessionStorage.setItem("hg_user", JSON.stringify(payload));
            }

            $("#msg")
                .removeClass("text-red-600")
                .addClass("text-green-600")
                .text("Đăng nhập thành công! Đang chuyển hướng...");

            console.log("ROLE TRẢ VỀ:", info.role);

            // ✅ Điều hướng theo role
            setTimeout(() => {
                if (info.role?.toLowerCase() === "admin")
                    window.location.href = "/Admin/Dashboard";
                else
                    window.location.href = "/App/Index";
            }, 800);
        },
        error: function (xhr) {
            let msg = "";

            try {
                const data = xhr.responseJSON || JSON.parse(xhr.responseText || "{ }");

                // ✅ Xử lý lỗi có mã cụ thể
                if (xhr.status === 401)
                    msg = "Sai mật khẩu. Vui lòng kiểm tra lại.";
                else if (xhr.status === 404)
                    msg = "Tài khoản không tồn tại trong hệ thống.";
                else if (xhr.status === 403)
                    msg = "Tài khoản của bạn chưa được kích hoạt hoặc đã bị khóa.";
                else if (typeof data === "string" && data.trim() !== "")
                    msg = data;
                else if (data?.message)
                    msg = data.message;
                else if (data?.detail)
                    msg = data.detail;
                else if (data?.errors)
                    msg = Object.values(data.errors).flat().join("\n");
            } catch (e) {
                // ignore parse error
            }

            // ✅ Nếu vẫn chưa có gì → fallback cuối cùng
            if (!msg || msg.trim() === "")
                msg = "Đăng nhập thất bại. Vui lòng thử lại.";

            $("#msg")
                .removeClass("hidden text-green-600")
                .addClass("text-red-600")
                .text(msg);
        },
        complete: () => $("#btnLogin").prop("disabled", false).text("Đăng nhập")
    });
});
