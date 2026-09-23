'use strict';
document.querySelectorAll('.menu-toggle,.menu-overlay').forEach(b=>b.addEventListener('click',()=>{if(innerWidth<=980){document.body.classList.toggle('menu-aberto');}else{document.body.classList.toggle('sidebar-recolhida');}document.querySelector('.menu-toggle')?.setAttribute('aria-expanded',String(document.body.classList.contains('menu-aberto')));}));
function inicializarPagina() {
document.querySelectorAll('.fechar-toast').forEach(b=>b.addEventListener('click',()=>b.closest('.toast-meta').remove()));
document.querySelectorAll('[data-moeda]').forEach(input=>{const formatar=()=>{let v=input.value.trim();if(!v)return;if(v.includes(','))v=v.replace(/\./g,'').replace(',','.');const n=Number(v);if(Number.isFinite(n)&&n>=0)input.value=n.toLocaleString('pt-BR',{minimumFractionDigits:2,maximumFractionDigits:2});};formatar();input.addEventListener('blur',formatar);});
document.querySelectorAll('[data-filtro]').forEach(b=>b.addEventListener('click',()=>{document.querySelectorAll('[data-filtro]').forEach(x=>x.classList.toggle('ativo',x===b));let visiveis=0;document.querySelectorAll('[data-status]').forEach(c=>{c.hidden=b.dataset.filtro!=='Todos'&&c.dataset.status!==b.dataset.filtro;if(!c.hidden)visiveis++;});document.querySelector('#vazio-filtro').hidden=visiveis>0;}));
const botaoSimular=document.querySelector('#simular');
botaoSimular?.addEventListener('click',async()=>{const form=document.querySelector('#form-objetivo');const resultado=document.querySelector('#resultado-simulacao');botaoSimular.disabled=true;resultado.textContent='Calculando seu plano...';try{const resposta=await fetch('/Objetivos/Simular',{method:'POST',body:new FormData(form)});if(!resposta.ok)throw Error('Confira os valores e as datas para calcular seu plano.');const c=await resposta.json();resultado.replaceChildren();const moeda=v=>v.toLocaleString('pt-BR',{style:'currency',currency:'BRL'});for(const [rotulo,valor] of [['Falta conquistar',c.restante],['Por mês',c.mensal],['A cada 15 dias',c.quinzenal],['Por semana',c.semanal]]){const linha=document.createElement('div');linha.className='entre';const texto=document.createElement('span');texto.textContent=rotulo;const numero=document.createElement('strong');numero.textContent=moeda(valor);linha.append(texto,numero);resultado.append(linha);}const obs=document.createElement('p');obs.className='muted mt-3';obs.textContent=c.atrasado?'O prazo venceu. Escolha uma nova data.':`${c.periodos} períodos restantes na frequência escolhida.`;resultado.append(obs);}catch(e){resultado.textContent=e.message;}finally{botaoSimular.disabled=false;}});

}
inicializarPagina();
document.addEventListener("app:navigated", inicializarPagina);
document.addEventListener("submit", event => {
 if(event.target.dataset.confirm && !confirm(event.target.dataset.confirm)) event.preventDefault();
});

// Navegacao incluida no ponto de entrada comum a todas as versoes do layout.
(() => {
  "use strict";
  const page = document.getElementById("app-page") || document.querySelector("main.conteudo");
  if (!page || !window.fetch || window.metaNavigationReady) return;
  window.metaNavigationReady = true;
  page.id = "app-page";
  page.tabIndex = -1;
  let busy = false;
  let currentUrl = location.href;
  let pendingPop = false;
  history.replaceState({ app: true }, "", currentUrl);

  const internal = url => url.origin === location.origin &&
    !/^\/(?:Login|Cadastro|EsqueciSenha|Sair)(?:\/|$)/i.test(url.pathname);

  async function navigate(url, { method = "GET", body, pop = false } = {}) {
    if (busy) return;
    busy = true;
    page.setAttribute("aria-busy", "true");
    document.body.classList.add("app-loading");
    const controls = [...document.querySelectorAll('button[type="submit"], input[type="submit"]')];
    const enabled = controls.filter(control => !control.disabled);
    enabled.forEach(control => control.disabled = true);
    document.getElementById("navigation-error")?.remove();
    try {
      const response = await fetch(url, { method, body, credentials: "same-origin",
        headers: { "X-Requested-With": "fetch" }, cache: "no-store" });
      const finalUrl = new URL(response.url);
      if (!internal(finalUrl)) { location.assign(finalUrl.href); return; }
      if (!response.ok) throw new Error("HTTP " + response.status);
      if (!response.headers.get("content-type")?.includes("text/html")) {
        throw new Error("Resposta inesperada");
      }
      const next = new DOMParser().parseFromString(await response.text(), "text/html");
      const nextPage = next.getElementById("app-page") || next.querySelector("main.conteudo");
      if (!nextPage) throw new Error("Conteudo de navegacao ausente");
      // Novas telas com scripts proprios devem declarar seu ciclo de inicializacao.
      if (nextPage.querySelector("script")) throw new Error("Script de pagina nao suportado");
      const modal = document.querySelector(".modal.show");
      if (modal) {
        await new Promise(resolve => {
          modal.addEventListener("hidden.bs.modal", resolve, { once: true });
          bootstrap.Modal.getOrCreateInstance(modal).hide();
        });
      }
      document.querySelectorAll('[data-bs-toggle="dropdown"]').forEach(element =>
        window.bootstrap?.Dropdown.getInstance(element)?.hide());
      if (pendingPop) return;
      page.replaceChildren(...nextPage.childNodes);
      document.title = next.title;
      const headerTitle = document.querySelector(".breadcrumb-text strong");
      const nextTitle = next.querySelector(".breadcrumb-text strong");
      if (headerTitle && nextTitle) headerTitle.textContent = nextTitle.textContent;
      const activeLinks = new Set([...next.querySelectorAll(".sidebar a.ativo")]
        .map(link => link.getAttribute("href")));
      document.querySelectorAll(".sidebar a.nav-item").forEach(link => {
        const active = activeLinks.has(link.getAttribute("href"));
        link.classList.toggle("ativo", active);
        if (active) link.setAttribute("aria-current", "page");
        else link.removeAttribute("aria-current");
      });
      const token = next.querySelector('#modalExclusao input[name="__RequestVerificationToken"]');
      const existingToken = document.querySelector('#modalExclusao input[name="__RequestVerificationToken"]');
      if (token && existingToken) existingToken.value = token.value;
      if (pendingPop) return;
      const destination = finalUrl.href;
      if (pop || destination === currentUrl) history.replaceState({ app: true }, "", destination);
      else history.pushState({ app: true }, "", destination);
      currentUrl = destination;
      document.body.classList.remove("menu-aberto");
      document.querySelector(".menu-toggle")?.setAttribute("aria-expanded", "false");
      document.dispatchEvent(new CustomEvent("app:navigated"));
      page.focus({ preventScroll: true });
      window.scrollTo(0, 0);
    } catch (error) {
      // Nunca repetir POST automaticamente: a operacao pode ter sido concluida.
      const alert = document.createElement("div");
      alert.id = "navigation-error";
      alert.className = "alert alert-danger";
      alert.setAttribute("role", "alert");
      alert.textContent = method === "POST"
        ? "Não foi possível confirmar o resultado. Consulte a listagem antes de enviar novamente."
        : "Não foi possível carregar a página. Tente novamente.";
      page.prepend(alert);
      if (pop && !pendingPop) history.replaceState({ app: true }, "", currentUrl);
    } finally {
      busy = false;
      enabled.forEach(control => control.disabled = false);
      page.removeAttribute("aria-busy");
      document.body.classList.remove("app-loading");
      if (pendingPop) { pendingPop = false; navigate(location.href, { pop: true }); }
    }
  }

  document.addEventListener("click", event => {
    if (event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey) return;
    const link = event.target.closest("a[href]");
    if (!link || link.hasAttribute("download") || link.dataset.noSpa !== undefined ||
        (link.target && link.target !== "_self") || link.hasAttribute("data-bs-toggle")) return;
    const url = new URL(link.href);
    if (!internal(url) || url.hash) return;
    event.preventDefault();
    navigate(url.href);
  });

  document.addEventListener("submit", event => {
    if (event.defaultPrevented) return;
    const form = event.target;
    const submitter = event.submitter;
    const url = new URL(submitter?.getAttribute("formaction") || form.action || location.href, location.href);
    const method = (submitter?.getAttribute("formmethod") || form.method || "GET").toUpperCase();
    if (!internal(url) || !["GET", "POST"].includes(method) || form.dataset.noSpa !== undefined ||
        (form.target && form.target !== "_self")) return;
    event.preventDefault();
    if (busy) return;
    const data = new FormData(form);
    if (submitter?.name) data.append(submitter.name, submitter.value);
    if (method === "GET") {
      url.search = new URLSearchParams(data).toString();
      navigate(url.href);
    } else {
      navigate(url.href, { method, body: data });
    }
  });

  window.addEventListener("popstate", () => {
    if (busy) { pendingPop = true; return; }
    navigate(location.href, { pop: true });
  });
})();