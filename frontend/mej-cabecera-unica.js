/* Mente en Juego: cabecera única. No modifica autenticación ni endpoints. */
(function () {
  function montarCabecera() {
    if (document.body.dataset.mejHeader === 'true') return;
    const rutas = ['panel_tecnico.html','area_medica.html','gestion_administrador.html','inicio_jugadora.html','formulario_jugadora.html'];
    const pagina = location.pathname.split('/').pop().toLowerCase();
    if (!rutas.includes(pagina)) return;
    const original = pagina === 'inicio_jugadora.html' || pagina === 'formulario_jugadora.html'
      ? document.querySelector('header') : document.querySelector('nav');
    if (!original) return;
    const usuarioOriginal = document.getElementById('usuarioBarra');
    const especialidadOriginal = document.getElementById('lblPerfilEspecialidad');
    const cabecera = document.createElement('header');
    cabecera.id='mej-header-global';
    cabecera.className='mej-header-global';
    cabecera.innerHTML = `<div class="mej-header-inner">
      <div class="mej-header-brand"><img class="mej-header-escudo" src="assets/logo_talleres.png" alt="Escudo de Talleres"><span class="mej-header-nombre">MENTE EN JUEGO</span></div>
      <div class="mej-header-actions"><span class="mej-header-usuario" id="mej-usuario-contenedor"></span><span id="mej-especialidad-contenedor"></span><button type="button" class="mej-header-salir" id="mej-salir-unificado">CERRAR SESIÓN</button></div>
    </div>`;
    // Reutilizar el mismo nodo de usuario: preserva actualizaciones desde los scripts existentes.
    const destinoUsuario = cabecera.querySelector('#mej-usuario-contenedor');
    if(usuarioOriginal) destinoUsuario.appendChild(usuarioOriginal);
    else destinoUsuario.textContent=localStorage.getItem('nombreUsuario') || 'Usuario conectado';
    if(especialidadOriginal) cabecera.querySelector('#mej-especialidad-contenedor').appendChild(especialidadOriginal);
    const oldLogout = original.querySelector('[onclick*="cerrarSesion"]');
    const logout = cabecera.querySelector('#mej-salir-unificado');
    logout.addEventListener('click',function(){
      if(typeof window.cerrarSesion==='function') window.cerrarSesion();
      else if(oldLogout) oldLogout.click();
    });
    original.replaceWith(cabecera);
    document.body.dataset.mejHeader='true';
  }
  if(document.readyState==='loading') document.addEventListener('DOMContentLoaded',montarCabecera);
  else montarCabecera();
})();
