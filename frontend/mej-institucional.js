/* Normalización visual: no altera permisos, llamadas API ni eventos existentes. */
(function(){'use strict';
function iniciar(){
  if(document.body.classList.contains('mej-login'))return;
  const h=document.querySelector('body > nav, body > header.encabezado');
  if(!h)return;
  h.classList.add('mej-header');
  if(h.matches('header.encabezado')){
    const title=h.querySelector('.titulo-sistema'); if(title)title.classList.add('mej-brand');
    const btn=h.querySelector('button[onclick*="cerrarSesion"]');if(btn)btn.classList.add('mej-session');
    return;
  }
  let brand=h.querySelector('.nav-marca');
  if(!brand){brand=h.querySelector('div');}
  if(brand){brand.classList.add('mej-brand');
    if(!brand.querySelector('img')){const img=document.createElement('img');img.src='assets/logo_talleres.png';img.alt='Escudo Talleres';brand.insertBefore(img,brand.firstChild);}
  }
  const salir=h.querySelector('button[onclick*="cerrarSesion"]');
  if(salir){const parent=salir.parentElement;if(parent&&parent!==h)parent.classList.add('mej-session');else salir.classList.add('mej-session');}
}
if(document.readyState==='loading')document.addEventListener('DOMContentLoaded',iniciar);else iniciar();
})();
