document.addEventListener("DOMContentLoaded", () => {

    const form = document.querySelector("form");

    const button = document.querySelector(".btn-auth");

    const inputs = document.querySelectorAll(".form-control");

    inputs.forEach(input => {

        input.addEventListener("focus", () => {

            input.parentElement.classList.add("active");

        });

        input.addEventListener("blur", () => {

            input.parentElement.classList.remove("active");

        });

    });

    form.addEventListener("submit", () => {

        button.disabled = true;

        button.innerHTML = `
            <span class="spinner"></span>
            Connexion...
        `;

    });

});