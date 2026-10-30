
/* =====================================================
   MENTE EN JUEGO
   Sistema global de confirmaciones y avisos
   ===================================================== */

(function () {
  "use strict";

  let cola = Promise.resolve();

  function abrirModalMEJ(mensaje, tipo = "info") {
    const ejecutar = () =>
      new Promise((resolver) => {
        const fondo = document.createElement("div");

        fondo.setAttribute("role", "presentation");

        fondo.style.cssText = `
          position: fixed;
          inset: 0;
          background: rgba(3, 15, 35, 0.68);
          z-index: 2147483647;
          display: flex;
          align-items: center;
          justify-content: center;
          padding: 16px;
        `;

        const dialogo = document.createElement("div");

        dialogo.setAttribute("role", "dialog");
        dialogo.setAttribute("aria-modal", "true");

        dialogo.style.cssText = `
          box-sizing: border-box;
          width: min(100%, 440px);
          background: #ffffff;
          color: #1f2937;
          border-radius: 14px;
          padding: 26px;
          box-shadow: 0 20px 55px rgba(0,0,0,.25);
          font-family: Segoe UI, Arial, sans-serif;
          text-align: center;
        `;

        const configuracion = {
          confirmar: {
            titulo: "CONFIRMAR OPERACIÓN",
            color: "#0b4dad"
          },
          exito: {
            titulo: "OPERACIÓN REALIZADA",
            color: "#15803d"
          },
          error: {
            titulo: "NO SE PUDO COMPLETAR",
            color: "#dc2626"
          },
          alerta: {
            titulo: "ATENCIÓN",
            color: "#b45309"
          },
          info: {
            titulo: "INFORMACIÓN",
            color: "#0b4dad"
          }
        };

        const config =
          configuracion[tipo] || configuracion.info;

        const titulo = document.createElement("h2");

        titulo.textContent = config.titulo;

        titulo.style.cssText = `
          color: ${config.color};
          font-size: 18px;
          font-weight: 800;
          margin: 0 0 14px;
        `;

        const detalle = document.createElement("p");

        detalle.textContent = String(mensaje ?? "");

        detalle.style.cssText = `
          font-size: 14px;
          line-height: 1.5;
          white-space: pre-wrap;
          margin: 0 0 22px;
        `;

        const acciones = document.createElement("div");

        acciones.style.cssText = `
          display: flex;
          gap: 12px;
          justify-content: center;
          flex-wrap: wrap;
        `;

        function cerrar(valor) {
          document.removeEventListener("keydown", teclado);
          fondo.remove();
          resolver(valor);
        }

        function teclado(evento) {
          if (evento.key === "Escape") {
            evento.preventDefault();
            cerrar(false);
          }
        }

        function crearBoton(texto, primario, valor) {
          const boton = document.createElement("button");

          boton.type = "button";
          boton.textContent = texto;

          boton.style.cssText = `
            min-width: 115px;
            padding: 11px 18px;
            border-radius: 8px;
            font-weight: 800;
            font-size: 13px;
            cursor: pointer;
            border: 1px solid #0b4dad;
            background: ${primario ? "#0b4dad" : "#fff"};
            color: ${primario ? "#fff" : "#0b4dad"};
          `;

          boton.addEventListener("click", () => {
            cerrar(valor);
          });

          acciones.appendChild(boton);

          return boton;
        }

        document.addEventListener("keydown", teclado);

        dialogo.append(titulo, detalle, acciones);
        fondo.appendChild(dialogo);
        document.body.appendChild(fondo);

        if (tipo === "confirmar") {
          crearBoton("CANCELAR", false, false);
        }

        const aceptar = crearBoton(
          tipo === "confirmar" ? "CONFIRMAR" : "ACEPTAR",
          true,
          true
        );

        aceptar.focus();
      });

    const actual = cola.then(ejecutar, ejecutar);

    cola = actual.then(
      () => {},
      () => {}
    );

    return actual;
  }

  // Confirmación: devuelve true o false.
  window.confirmarMEJ = function (mensaje) {
    return abrirModalMEJ(mensaje, "confirmar");
  };

  // Aviso de operación exitosa.
  window.mostrarExitoMEJ = function (mensaje) {
    return abrirModalMEJ(mensaje, "exito");
  };

  // Aviso de error.
  window.mostrarErrorMEJ = function (mensaje) {
    return abrirModalMEJ(mensaje, "error");
  };

  // Aviso informativo.
  window.mostrarInfoMEJ = function (mensaje) {
    return abrirModalMEJ(mensaje, "info");
  };

  // Aviso de advertencia.
  window.mostrarAlertaMEJ = function (mensaje) {
    return abrirModalMEJ(mensaje, "alerta");
  };

  // Compatibilidad con llamadas existentes.
  window.mostrarAvisoMEJ = function (
    tipo,
    titulo,
    mensaje
  ) {
    const tiposValidos = [
      "exito",
      "error",
      "alerta",
      "info",
      "confirmar"
    ];

    // Admite mostrarAvisoMEJ("Mensaje")
    if (arguments.length === 1) {
      return abrirModalMEJ(tipo, "info");
    }

    // Admite mostrarAvisoMEJ("exito", "Mensaje")
    if (arguments.length === 2) {
      if (tiposValidos.includes(tipo)) {
        return abrirModalMEJ(titulo, tipo);
      }

      return abrirModalMEJ(
        `${tipo}\n${titulo}`,
        "info"
      );
    }

    const tipoFinal = tiposValidos.includes(tipo)
      ? tipo
      : "info";

    const texto = [titulo, mensaje]
      .filter((valor) => valor != null && valor !== "")
      .join("\n\n");

    return abrirModalMEJ(texto, tipoFinal);
  };

})();

/* MENTE EN JUEGO - Solicitud de datos reutilizable */

window.solicitarDatoMEJ = function (
  mensaje,
  valorInicial = ""
) {
  return new Promise((resolver) => {
    const fondo = document.createElement("div");

    fondo.style.cssText = `
      position: fixed;
      inset: 0;
      background: rgba(3,15,35,.68);
      z-index: 2147483647;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 16px;
    `;

    const modal = document.createElement("div");

    modal.setAttribute("role", "dialog");
    modal.setAttribute("aria-modal", "true");

    modal.style.cssText = `
      width: min(100%, 440px);
      box-sizing: border-box;
      background: white;
      border-radius: 14px;
      padding: 26px;
      box-shadow: 0 20px 55px rgba(0,0,0,.25);
      font-family: Segoe UI, Arial, sans-serif;
      text-align: center;
    `;

    const titulo = document.createElement("h2");
    titulo.textContent = "INGRESAR DATO";
    titulo.style.cssText = `
      color: #0b4dad;
      font-size: 18px;
      margin: 0 0 15px;
    `;

    const texto = document.createElement("p");
    texto.textContent = String(mensaje ?? "");
    texto.style.cssText = `
      font-size: 14px;
      color: #334155;
      margin-bottom: 18px;
    `;

    const campo = document.createElement("input");
    campo.type = "text";
    campo.value = String(valorInicial ?? "");
    campo.style.cssText = `
      width: 100%;
      box-sizing: border-box;
      padding: 12px;
      border: 1px solid #cbd5e1;
      border-radius: 8px;
      margin-bottom: 20px;
      font-size: 14px;
    `;

    const acciones = document.createElement("div");
    acciones.style.cssText = `
      display: flex;
      justify-content: center;
      gap: 12px;
      flex-wrap: wrap;
    `;

    function cerrar(resultado) {
      document.removeEventListener("keydown", teclado);
      fondo.remove();
      resolver(resultado);
    }

    function teclado(evento) {
      if (evento.key === "Escape") {
        cerrar(null);
      }

      if (evento.key === "Enter") {
        cerrar(campo.value);
      }
    }

    function boton(textoBoton, primario, accion) {
      const b = document.createElement("button");
      b.type = "button";
      b.textContent = textoBoton;

      b.style.cssText = `
        padding: 11px 18px;
        min-width: 115px;
        border-radius: 8px;
        cursor: pointer;
        font-weight: 800;
        border: 1px solid #0b4dad;
        background: ${primario ? "#0b4dad" : "#fff"};
        color: ${primario ? "#fff" : "#0b4dad"};
      `;

      b.addEventListener("click", accion);
      acciones.appendChild(b);
    }

    boton("CANCELAR", false, () => cerrar(null));

    boton("ACEPTAR", true, () => {
      cerrar(campo.value);
    });

    modal.append(titulo, texto, campo, acciones);
    fondo.appendChild(modal);
    document.body.appendChild(fondo);

    document.addEventListener("keydown", teclado);

    campo.focus();
  });
};

