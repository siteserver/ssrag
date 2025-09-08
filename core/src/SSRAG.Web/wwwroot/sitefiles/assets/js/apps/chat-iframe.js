const chatWindowId = "ssrag-chatbot-window";
let btnOpen;
let chatBtnDiv;
let btnId;

function embedChatbot() {
  const script = document.getElementById("chat-iframe");
  const defaultOpen = script?.getAttribute("data-default-open") === "true";

  const btnDraggable = script?.getAttribute("data-btn-draggable") === "true";
  btnId = script?.getAttribute("data-btn-id");
  btnOpen = script?.getAttribute("data-btn-open") || `/sitefiles/assets/images/apps/chat-open.png`;
  const btnClose =
    script?.getAttribute("data-btn-close") || "/sitefiles/assets/images/apps/chat-close.png";
  const btnStyle = script?.getAttribute("data-btn-style");
  const winSrc = script?.getAttribute("data-win-src");
  const winStyle = script?.getAttribute("data-win-style");

  if (!winSrc) {
    console.error(`chat-iframe script tag must have a data-win-src attribute`);
    return;
  }

  chatBtnDiv = document.createElement("img");
  let chatBtn = document.createElement("div");
  if (btnId) {
    chatBtn = document.getElementById(btnId);
  } else {
    chatBtn.style.cssText =
      btnStyle + "position: fixed; cursor: pointer; z-index: 2147483647; transition: 0;";

    // btn icon
    chatBtnDiv.src = defaultOpen ? btnClose : btnOpen;
    chatBtnDiv.setAttribute("width", "100%");
    chatBtnDiv.setAttribute("height", "100%");
    chatBtnDiv.draggable = false;
  }

  const iframe = document.createElement("iframe");
  iframe.allow = "*";
  iframe.referrerPolicy = "no-referrer";
  iframe.title = "SSRAG Chat Window";
  iframe.id = chatWindowId;
  iframe.src = winSrc;
  iframe.style.cssText =
    winStyle +
    "border: none; position: fixed; flex-direction: column; justify-content: space-between; box-shadow: rgba(150, 150, 150, 0.2) 0px 10px 30px 0px, rgba(150, 150, 150, 0.2) 0px 0px 0px 1px; border-radius: 12px; display: flex; z-index: 2147483648; overflow: hidden; left: unset; background-color: #F3F4F6;";
  iframe.style.visibility = defaultOpen ? "unset" : "hidden";

  document.body.appendChild(iframe);

  let chatBtnDragged = false;
  let chatBtnDown = false;
  let chatBtnMouseX;
  let chatBtnMouseY;

  chatBtn.addEventListener("click", function (e) {
    e.stopPropagation();
    e.preventDefault();
    if (chatBtnDragged) {
      chatBtnDragged = false;
      return;
    }
    const chatWindow = document.getElementById(chatWindowId);

    if (!chatWindow) return;
    const visibilityVal = chatWindow.style.visibility;
    if (visibilityVal === "hidden") {
      chatWindow.style.visibility = "unset";
      if (!btnId) {
        chatBtnDiv.src = btnClose;
      }
    } else {
      chatWindow.style.visibility = "hidden";
      if (!btnId) {
        chatBtnDiv.src = btnOpen;
      }
    }
  });

  chatBtn.addEventListener("mousedown", (e) => {
    e.stopPropagation();

    if (!chatBtnMouseX && !chatBtnMouseY) {
      chatBtnMouseX = e.clientX;
      chatBtnMouseY = e.clientY;
    }

    chatBtnDown = true;
  });

  window.addEventListener("mousemove", (e) => {
    e.stopPropagation();
    if (!btnDraggable || !chatBtnDown) return;

    chatBtnDragged = true;
    const transformX = e.clientX - chatBtnMouseX;
    const transformY = e.clientY - chatBtnMouseY;

    if (!btnId) {
      chatBtn.style.transform = `translate3d(${transformX}px, ${transformY}px, 0)`;
    }
  });

  window.addEventListener("mouseup", (e) => {
    chatBtnDown = false;
  });

  chatBtn.appendChild(chatBtnDiv);
  document.body.appendChild(chatBtn);
}

function closeChatbot() {
  const chatWindow = document.getElementById(chatWindowId);
  chatWindow.style.visibility = "hidden";
  if (!btnId) {
    chatBtnDiv.src = btnOpen;
  }
}

window.addEventListener("load", function () {
  setTimeout(() => {
    embedChatbot();
  }, 1000);
});

window.addEventListener(
  "message",
  function (event) {
    if (event && event.data && event.data.type === "closeChatbot") {
      closeChatbot();
    }
  },
  false
);
