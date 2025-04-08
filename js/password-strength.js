$(document).ready(function () {
    // Show password requirements when password field is focused
    $("#password").on("focus", function () {
        $("#password-requirements").fadeIn(200);
    });

    // Hide password requirements when clicking outside
    $(document).on("click", function (e) {
        if (!$(e.target).closest("#password, #password-requirements").length) {
            $("#password-requirements").fadeOut(200);
        }
    });

    // Toggle password visibility
    $("#togglePassword").click(function () {
        var passwordField = $("#password");
        var icon = $(this).find("i");

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        } else {
            passwordField.attr("type", "password");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        }
    });

    // Toggle confirm password visibility
    $("#toggleConfirmPassword").click(function () {
        var passwordField = $("#confirmPassword");
        var icon = $(this).find("i");

        if (passwordField.attr("type") === "password") {
            passwordField.attr("type", "text");
            icon.removeClass("bi-eye").addClass("bi-eye-slash");
        } else {
            passwordField.attr("type", "password");
            icon.removeClass("bi-eye-slash").addClass("bi-eye");
        }
    });

    // Password strength and requirements checker
    $("#password").on("keyup", function () {
        var password = $(this).val();
        var strength = 0;
        var strengthText = "";
        var strengthClass = "bg-danger";

        // Update requirements
        // 1. Length requirement (8-20 characters)
        if (password.length >= 8 && password.length <= 20) {
            $("#req-length").removeClass("text-danger").addClass("text-success");
            $("#req-length i").removeClass("bi-x-circle").addClass("bi-check-circle");
            strength += 25;
        } else {
            $("#req-length").removeClass("text-success").addClass("text-danger");
            $("#req-length i").removeClass("bi-check-circle").addClass("bi-x-circle");
        }

        // 2. Capital letter requirement
        if (/[A-Z]/.test(password)) {
            $("#req-capital").removeClass("text-danger").addClass("text-success");
            $("#req-capital i").removeClass("bi-x-circle").addClass("bi-check-circle");
            strength += 25;
        } else {
            $("#req-capital").removeClass("text-success").addClass("text-danger");
            $("#req-capital i").removeClass("bi-check-circle").addClass("bi-x-circle");
        }

        // 3. Number requirement
        if (/\d/.test(password)) {
            $("#req-number").removeClass("text-danger").addClass("text-success");
            $("#req-number i").removeClass("bi-x-circle").addClass("bi-check-circle");
            strength += 25;
        } else {
            $("#req-number").removeClass("text-success").addClass("text-danger");
            $("#req-number i").removeClass("bi-check-circle").addClass("bi-x-circle");
        }

        // 4. No spaces requirement
        if (!/\s/.test(password)) {
            $("#req-spaces").removeClass("text-danger").addClass("text-success");
            $("#req-spaces i").removeClass("bi-x-circle").addClass("bi-check-circle");
            strength += 25;
        } else {
            $("#req-spaces").removeClass("text-success").addClass("text-danger");
            $("#req-spaces i").removeClass("bi-check-circle").addClass("bi-x-circle");
        }

        // Additional strength bonuses
        if (password.length > 12) strength += 10;
        if (/[!#$%^&*(),.?":{}|<>]/.test(password)) strength += 15;

        // Cap strength at 100
        strength = Math.min(strength, 100);

        // Set strength meter color and text
        if (strength < 40) {
            strengthText = "Weak";
            strengthClass = "bg-danger";
        } else if (strength < 70) {
            strengthText = "Medium";
            strengthClass = "bg-warning";
        } else {
            strengthText = "Strong";
            strengthClass = "bg-success";
        }

        // Update the UI
        $("#password-strength-meter")
            .removeClass("bg-danger bg-warning bg-success")
            .addClass(strengthClass)
            .css("width", strength + "%")
            .attr("aria-valuenow", strength);

        $("#password-strength-text").text(strengthText);
    });

    // Phone number validation
    $("#PhoneNumber").on("keypress", function (e) {
        // Allow only digit keys and control keys (backspace, delete, arrows)
        if (e.which < 48 || e.which > 57) {
            if (e.which != 8 && e.which != 0 && e.which != 9) {
                e.preventDefault();
            }
        }
    });

    // Add submit handler to ensure form is properly submitted
    $("form").on("submit", function () {
        // Check if date of birth is selected
        if (!$("#DateOfBirth").val()) {
            alert("Please select your date of birth.");
            return false;
        }

        $("#registerButton").prop("disabled", true).html('<span class="spinner-border spinner-border-sm" role="status" aria-hidden="true"></span> Processing...');
        return true;
    });
});