(() => {
    "use strict";

    document.addEventListener("DOMContentLoaded", initializeBuyoraChat);

    function initializeBuyoraChat() {
        const widget = document.getElementById("buyoraChatWidget");
        if (!widget) return;

        const backdrop = document.getElementById("buyoraChatBackdrop");
        const panel = document.getElementById("buyoraChatPanel");
        const toggle = document.getElementById("buyoraChatToggle");
        const close = document.getElementById("buyoraChatClose");
        const newChat = document.getElementById("buyoraChatNew");
        const messages = document.getElementById("buyoraChatMessages");
        const welcome = widget.querySelector(".buyora-chat-welcome");
        const form = document.getElementById("buyoraChatForm");
        const input = document.getElementById("buyoraChatInput");
        const send = document.getElementById("buyoraChatSend");
        const status = document.getElementById("buyoraChatStatus");
        const language = widget.dataset.language === "en" ? "en" : "ar";
        const authenticated = widget.dataset.authenticated === "true";
        const strings = language === "en" ? {
            close: "Close chat", open: "Open Buyora chat", newChat: "Start a new chat", send: "Send message", suggestions: "Suggested questions",
            placeholder: "Type your message...", welcomeTitle: "Welcome to Buyora", welcomeBody: "Ask me about products, offers, or your orders.",
            title: "Buyora Assistant", online: "Here to help", deals: "Deals & discounts", orders: "Track my orders",
            footnote: "Buyora AI assistant · Don’t share sensitive information", toggle: "Need help?", inputLabel: "Type your message",
            empty: "Please type a message first.", failed: "Sorry, I couldn’t connect right now. Please try again in a moment.",
            responseFailed: "I couldn’t get a response right now. Please try again."
        } : {
            close: "إغلاق المحادثة", open: "افتح محادثة Buyora", newChat: "محادثة جديدة", send: "إرسال الرسالة", suggestions: "أسئلة مقترحة",
            placeholder: "اكتب رسالتك هنا...", welcomeTitle: "أهلًا بيك في Buyora", welcomeBody: "اسألني عن المنتجات، العروض أو طلباتك.",
            title: "مساعد Buyora", online: "متاح لمساعدتك", deals: "العروض والخصومات", orders: "متابعة طلباتي",
            footnote: "مساعد Buyora الذكي · لا تشارك بياناتك الحساسة", toggle: "محتاج مساعدة؟", inputLabel: "اكتب رسالتك",
            empty: "اكتب رسالتك الأول.", failed: "مش قادر أتصل بالمساعد دلوقتي. جرّب تاني بعد شوية.",
            responseFailed: "ماقدرتش أجيب رد دلوقتي. جرّب تاني."
        };

        widget.querySelectorAll("[data-chat-text]").forEach((element) => {
            const key = element.dataset.chatText;
            if (strings[key]) element.textContent = strings[key];
        });
        widget.querySelectorAll("[data-chat-aria]").forEach((element) => {
            const key = element.dataset.chatAria;
            if (strings[key]) element.setAttribute("aria-label", strings[key]);
        });
        input.placeholder = strings.placeholder;
        input.dir = "auto";

        const conversationStorageKey = `buyora-chat:${widget.dataset.chatScope || "guest"}`;
        const conversationTtl = 24 * 60 * 60 * 1000;
        let conversation = readConversation();
        let sessionId = conversation?.sessionId || createSessionId();
        let chatHistory = conversation?.messages || [];
        let customerToken = null;
        let tokenExpiresAt = 0;
        let isSending = false;
        let returnFocusTo = null;
        let expirationTimer = null;

        function createSessionId() {
            return window.crypto?.randomUUID?.() || `buyora-${Date.now()}-${Math.random().toString(36).slice(2)}`;
        }

        function readConversation() {
            try {
                const raw = window.localStorage.getItem(conversationStorageKey);
                if (!raw) return null;
                const saved = JSON.parse(raw);
                if (!saved || !saved.sessionId || !Number.isFinite(saved.updatedAt)
                    || Date.now() - saved.updatedAt > conversationTtl
                    || !Array.isArray(saved.messages)) {
                    window.localStorage.removeItem(conversationStorageKey);
                    return null;
                }
                saved.messages = saved.messages
                    .filter((message) => message && ["user", "assistant"].includes(message.role) && typeof message.text === "string")
                    .slice(-40);
                return saved;
            } catch {
                return null;
            }
        }

        function saveConversation() {
            try {
                const updatedAt = Date.now();
                window.localStorage.setItem(conversationStorageKey, JSON.stringify({
                    sessionId,
                    updatedAt,
                    messages: chatHistory.slice(-40)
                }));
                scheduleConversationExpiry(updatedAt + conversationTtl);
            } catch {
                // The chat still works when browser storage is unavailable.
            }
        }

        function scheduleConversationExpiry(expiresAt) {
            window.clearTimeout(expirationTimer);
            expirationTimer = window.setTimeout(() => {
                if (isSending) {
                    scheduleConversationExpiry(Date.now() + 60 * 1000);
                    return;
                }
                clearConversation();
            }, Math.max(0, expiresAt - Date.now()));
        }

        function clearConversation() {
            chatHistory = [];
            sessionId = createSessionId();
            customerToken = null;
            tokenExpiresAt = 0;
            messages.querySelectorAll(".buyora-chat-message").forEach((message) => message.remove());
            welcome.hidden = false;
            status.textContent = "";
            try { window.localStorage.removeItem(conversationStorageKey); } catch { /* Storage may be blocked. */ }
        }

        function openPanel() {
            returnFocusTo = document.activeElement;
            panel.hidden = false;
            panel.setAttribute("aria-hidden", "false");
            toggle.setAttribute("aria-expanded", "true");
            widget.classList.add("is-open");
            if (backdrop) backdrop.hidden = false;
            if (window.innerWidth < 768) {
                document.body.classList.add("buyora-chat-open");
            }
            scrollToLatest();
            window.setTimeout(() => input.focus(), 60);
        }

        function closePanel() {
            panel.hidden = true;
            panel.setAttribute("aria-hidden", "true");
            toggle.setAttribute("aria-expanded", "false");
            widget.classList.remove("is-open");
            if (backdrop) backdrop.hidden = true;
            document.body.classList.remove("buyora-chat-open");
            if (returnFocusTo && typeof returnFocusTo.focus === "function") returnFocusTo.focus();
        }

        function scrollToLatest() {
            messages.scrollTop = messages.scrollHeight;
        }

        function safeHref(href) {
            try {
                const url = new URL(href, window.location.origin);
                if (url.protocol !== "https:" && url.protocol !== "http:") return null;
                return { href: url.href, external: url.origin !== window.location.origin };
            } catch {
                return null;
            }
        }

        function renderInlineMarkdown(element, text) {
            // Normalize escaped/malformed Markdown links commonly returned by chat models.
            const normalizedText = String(text ?? "")
                .replace(/\\([()[\]])/g, "$1")
                .replace(/\]\(\s+/g, "](")
                .replace(/\s+\)/g, ")");
            const tokenPattern = /(\[[^\]]+\]\([^)]+\)|https?:\/\/[^\s<>\"']+|www\.[^\s<>\"']+|\/[A-Za-z0-9._~!$&'()*+,;=:@%/?#-]+|\*\*[^*]+\*\*|__[^_]+__|\*[^*\n]+\*|_[^_\n]+_)/g;
            let cursor = 0;
            let match;
            while ((match = tokenPattern.exec(normalizedText)) !== null) {
                element.appendChild(document.createTextNode(normalizedText.slice(cursor, match.index)));
                const token = match[0];
                const linkMatch = token.match(/^\[([^\]]+)\]\(\s*([^)]+?)\s*\)$/);
                const bold = (token.startsWith("**") && token.endsWith("**")) || (token.startsWith("__") && token.endsWith("__"));
                const italic = !bold && ((token.startsWith("*") && token.endsWith("*")) || (token.startsWith("_") && token.endsWith("_")));

                const isBareLink = !linkMatch && /^(https?:\/\/|www\.|\/)/i.test(token);
                if (linkMatch || isBareLink) {
                    const rawHref = linkMatch ? linkMatch[2].trim() : token;
                    const punctuation = isBareLink ? (rawHref.match(/[.,!?;،؛؟]+$/)?.[0] ?? "") : "";
                    let href = punctuation ? rawHref.slice(0, -punctuation.length) : rawHref;
                    if (/^www\./i.test(href)) href = `https://${href}`;
                    const safe = safeHref(href);
                    if (!safe) {
                        element.appendChild(document.createTextNode(token));
                    } else {
                        const link = document.createElement("a");
                        link.href = safe.href;
                        link.textContent = linkMatch ? linkMatch[1] : href;
                        if (safe.external) {
                            link.target = "_blank";
                            link.rel = "noopener noreferrer";
                        }
                        element.appendChild(link);
                        if (punctuation) element.appendChild(document.createTextNode(punctuation));
                    }
                } else if (bold || italic) {
                    const formatted = document.createElement(bold ? "strong" : "em");
                    const trimLength = bold ? 2 : 1;
                    renderInlineMarkdown(formatted, token.slice(trimLength, -trimLength));
                    element.appendChild(formatted);
                } else {
                    element.appendChild(document.createTextNode(token));
                }
                cursor = tokenPattern.lastIndex;
            }
            element.appendChild(document.createTextNode(normalizedText.slice(cursor)));
        }

        function renderAssistantText(element, text) {
            const normalized = text
                .replace(/\\+([*_\-])/g, "$1")
                .replace(/\\+\r?\n/g, "\n")
                .replace(/\\n/g, "\n")
                .replace(/\r\n?/g, "\n")
                .trim();
            let currentList = null;
            let currentListType = null;

            const flushList = () => {
                if (currentList) element.appendChild(currentList);
                currentList = null;
                currentListType = null;
            };

            normalized.split("\n").forEach((line) => {
                const bullet = line.match(/^\s*[-*•]\s+(.+)$/);
                const numbered = line.match(/^\s*\d+[.)]\s+(.+)$/);
                const listType = bullet ? "ul" : numbered ? "ol" : null;
                if (listType) {
                    if (currentListType !== listType) flushList();
                    if (!currentList) {
                        currentList = document.createElement(listType);
                        currentListType = listType;
                    }
                    const item = document.createElement("li");
                    renderInlineMarkdown(item, (bullet || numbered)[1]);
                    currentList.appendChild(item);
                    return;
                }

                flushList();
                if (!line.trim()) return;
                const paragraph = document.createElement("p");
                renderInlineMarkdown(paragraph, line.trim());
                element.appendChild(paragraph);
            });
            flushList();
        }

        function addMessage(text, kind, persist = true) {
            const bubble = document.createElement("div");
            bubble.className = `buyora-chat-message ${kind === "user" ? "is-user" : "is-assistant"}`;
            bubble.dir = "auto";
            if (kind === "assistant") renderAssistantText(bubble, text);
            else bubble.textContent = text;
            welcome.hidden = true;
            messages.appendChild(bubble);
            if (persist) {
                chatHistory.push({ role: kind, text });
                chatHistory = chatHistory.slice(-40);
                saveConversation();
            }
            scrollToLatest();
            return bubble;
        }

        function addTypingIndicator() {
            const bubble = document.createElement("div");
            bubble.className = "buyora-chat-message is-assistant buyora-chat-typing";
            bubble.setAttribute("aria-label", language === "en" ? "Assistant is typing" : "المساعد بيكتب");
            bubble.innerHTML = "<span></span><span></span><span></span>";
            messages.appendChild(bubble);
            scrollToLatest();
            return bubble;
        }

        async function getCustomerToken() {
            if (!authenticated) return null;
            if (customerToken && Date.now() < tokenExpiresAt) return customerToken;

            try {
                const response = await fetch(widget.dataset.sessionTokenUrl, {
                    method: "POST",
                    credentials: "same-origin",
                    cache: "no-store",
                    headers: { Accept: "application/json" }
                });
                if (!response.ok || response.redirected) return null;
                const result = await response.json();
                if (!result.token) return null;
                customerToken = result.token;
                tokenExpiresAt = Date.now() + Math.max(30, (result.expiresInSeconds || 600) - 30) * 1000;
                return customerToken;
            } catch {
                return null;
            }
        }

        function extractReply(data) {
            const result = Array.isArray(data) ? data[0] : data;
            const value = result?.output ?? result?.reply ?? result?.answer ?? result?.text ?? result;
            if (typeof value === "string") return value.trim();
            if (value && typeof value === "object") {
                const nested = value.output ?? value.reply ?? value.text;
                if (typeof nested === "string") return nested.trim();
                return JSON.stringify(value);
            }
            return "";
        }

        async function sendMessage(rawText) {
            const chatInput = rawText.trim();
            if (!chatInput || isSending) return;
            if (chatInput.length > 1000) return;

            status.textContent = "";
            addMessage(chatInput, "user");
            input.value = "";
            input.style.height = "auto";
            isSending = true;
            send.disabled = true;
            input.disabled = true;
            const typing = addTypingIndicator();

            try {
                const needsOrderAccess = /(طلب|طلبات|طلبي|طلباتي|تتبع|الشحنه|الشحنة|my\s+orders?|track|shipment)/i.test(chatInput);
                const token = needsOrderAccess ? await getCustomerToken() : null;
                const payload = { chatInput, sessionId };
                if (token) payload.customerToken = token;

                const response = await fetch(widget.dataset.webhookUrl, {
                    method: "POST",
                    mode: "cors",
                    headers: { "Content-Type": "application/json", Accept: "application/json" },
                    body: JSON.stringify(payload)
                });

                if (!response.ok) throw new Error(`Webhook returned ${response.status}`);
                const responseData = await response.json();
                typing.remove();
                const reply = extractReply(responseData);
                addMessage(reply || strings.responseFailed, "assistant");
            } catch {
                typing.remove();
                addMessage(strings.failed, "assistant");
            } finally {
                isSending = false;
                send.disabled = false;
                input.disabled = false;
                if (!panel.hidden) input.focus();
            }
        }

        if (chatHistory.length) {
            welcome.hidden = true;
            chatHistory.forEach((message) => addMessage(message.text, message.role, false));
            scheduleConversationExpiry(conversation.updatedAt + conversationTtl);
        }

        toggle.addEventListener("click", () => panel.hidden ? openPanel() : closePanel());
        close.addEventListener("click", closePanel);
        if (backdrop) {
            backdrop.addEventListener("click", closePanel);
        }

        // Close on clicking outside the chat widget
        document.addEventListener("pointerdown", (event) => {
            if (!panel.hidden && !widget.contains(event.target)) {
                closePanel();
            }
        });

        // Auto-scroll when virtual keyboard toggles on mobile
        if (window.visualViewport) {
            window.visualViewport.addEventListener("resize", () => {
                if (!panel.hidden && window.innerWidth < 768) {
                    scrollToLatest();
                }
            });
        }

        window.addEventListener("resize", () => {
            if (window.innerWidth >= 768 && document.body.classList.contains("buyora-chat-open")) {
                document.body.classList.remove("buyora-chat-open");
            }
        });
        newChat.addEventListener("click", () => {
            if (isSending) return;
            window.clearTimeout(expirationTimer);
            clearConversation();
            input.focus();
        });
        form.addEventListener("submit", (event) => {
            event.preventDefault();
            if (!input.value.trim()) {
                status.textContent = strings.empty;
                input.focus();
                return;
            }
            sendMessage(input.value);
        });
        input.addEventListener("input", () => {
            status.textContent = "";
            input.style.height = "auto";
            input.style.height = `${Math.min(input.scrollHeight, 110)}px`;
        });
        input.addEventListener("keydown", (event) => {
            if (event.key === "Enter" && !event.shiftKey && !event.isComposing) {
                event.preventDefault();
                form.requestSubmit();
            }
        });
        widget.querySelectorAll(".buyora-chat-suggestion").forEach((button) => {
            button.addEventListener("click", () => {
                const prompt = language === "en" ? button.dataset.chatPromptEn : button.dataset.chatPromptAr;
                if (prompt) sendMessage(prompt);
            });
        });
        document.addEventListener("keydown", (event) => {
            if (event.key === "Escape" && !panel.hidden) closePanel();
        });
    }
})();
