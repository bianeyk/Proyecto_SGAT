// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Valida las búsquedas antes de enviarlas para mostrar un aviso amigable
// sin reemplazar la página actual por una respuesta BadRequest.
(function () {
    // Evita que la rueda del mouse o el gesto del touchpad cambien
    // accidentalmente los valores de los campos numéricos.
    document.addEventListener("wheel", function (evento) {
        if (document.activeElement?.matches('input[type="number"]')) {
            evento.preventDefault();
        }
    }, { passive: false });

    const formulariosBusqueda = document.querySelectorAll("form.sg-buscador");
    if (!formulariosBusqueda.length) return;

    formulariosBusqueda.forEach(function (formulario) {
        formulario.addEventListener("submit", function (evento) {
            const entrada = formulario.querySelector('input[name="termino"]');
            if (!entrada) return;

            const termino = entrada.value.trim();
            if (termino.length === 0 || (termino.length >= 2 && termino.length <= 100)) {
                return;
            }

            evento.preventDefault();

            const mensaje = termino.length < 2
                ? "El término de búsqueda debe tener al menos 2 caracteres."
                : "El término de búsqueda no puede superar los 100 caracteres.";

            mostrarAvisoBusqueda(mensaje);
            entrada.focus();
        });
    });

    function mostrarAvisoBusqueda(mensaje) {
        const avisoAnterior = document.getElementById("avisoBusqueda");
        avisoAnterior?.remove();

        const aviso = document.createElement("div");
        aviso.id = "avisoBusqueda";
        aviso.className =
            "alert alert-warning alert-dismissible fade show position-fixed top-0 end-0 m-3 shadow";
        aviso.setAttribute("role", "alert");
        aviso.style.zIndex = "1090";
        aviso.innerHTML =
            `<i class="bi bi-exclamation-triangle me-2"></i>${mensaje}` +
            '<button type="button" class="btn-close" aria-label="Cerrar"></button>';

        aviso.querySelector(".btn-close").addEventListener("click", function () {
            aviso.remove();
        });

        document.body.appendChild(aviso);
        window.setTimeout(function () {
            aviso.remove();
        }, 4000);
    }
})();
