(() => {
    const boton = document.getElementById("buscarClienteBoton");
    if (!boton) return;
    const entrada = document.getElementById("buscarCliente");
    const lista = document.getElementById("IdCliente");
    const mensaje = document.getElementById("mensajeClientes");
    async function buscar() {
        const termino = entrada.value.trim().replace(/\s+/g, " ");
        if (termino.length < 2 || termino.length > 100) {
            mensaje.textContent = "Escribe entre 2 y 100 caracteres.";
            entrada.focus();
            return;
        }
        boton.disabled = true;
        mensaje.textContent = "Buscando clientes…";
        try {
            const respuesta = await fetch(`${boton.dataset.url}?termino=${encodeURIComponent(termino)}`, {
                headers: { "Accept": "application/json" }
            });
            if (!respuesta.ok || !respuesta.headers.get("content-type")?.includes("application/json"))
                throw new Error(respuesta.status === 429 ? "Demasiadas búsquedas. Espera un minuto." : "No se pudieron consultar los clientes. Revisa tu sesión e inténtalo de nuevo.");
            const clientes = await respuesta.json();
            const seleccionado = lista.selectedOptions[0];
            const valor = lista.value;
            lista.replaceChildren(new Option("Selecciona el cliente", ""));
            if (valor && seleccionado) lista.add(new Option(seleccionado.text, valor));
            for (const cliente of clientes) {
                if (String(cliente.id) !== valor) lista.add(new Option(cliente.nombre, cliente.id));
            }
            lista.value = valor;
            mensaje.textContent = clientes.length ? `${clientes.length} coincidencias (máximo 20). Selecciona un cliente; afina la búsqueda si no aparece.` : "No se encontraron clientes activos.";
        } catch (error) {
            mensaje.textContent = error.message;
        } finally {
            boton.disabled = false;
        }
    }
    boton.addEventListener("click", buscar);
    entrada.addEventListener("keydown", event => {
        if (event.key === "Enter") {
            event.preventDefault();
            if (!boton.disabled) buscar();
        }
    });

})();
