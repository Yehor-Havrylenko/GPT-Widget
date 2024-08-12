const chatContainer = document.getElementById('chat-container');
const chatToggleButton = document.getElementById('chat-toggle-button');
const chatMessages = document.getElementById('chat-messages');
const userInput = document.getElementById('user-input');
const sendButton = document.getElementById('send-button');

// Генерация уникального идентификатора сессии
function generateSessionId() {
    return '_' + Math.random().toString(36).substr(2, 9);
}

// Получение session_id (если сессия уже существует, используем её)
let sessionId = localStorage.getItem('sessionId');
if (!sessionId) {
    sessionId = generateSessionId();
    localStorage.setItem('sessionId', sessionId);
}

// Показ/скрытие чата
chatToggleButton.addEventListener('click', () => {
    chatContainer.classList.toggle('show');
});

sendButton.addEventListener('click', async () => {
  const userMessage = userInput.value;
  if (userMessage.trim() !== '') {
    addChatMessage('user', userMessage);
    userInput.value = '';

    try {
      const response = await fetch('http://localhost:5270/chat/sendmessage', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          session_id: sessionId,  // Передаем session_id на сервер
          message: userMessage
        })
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      const decodedMessage = data.response_text;

      addChatMessage('bot', decodedMessage);
    } catch (error) {
      console.error('Ошибка при отправке сообщения:', error);
      addChatMessage('bot', 'Произошла ошибка. Попробуйте позже.');
    }
  }
});

function addChatMessage(sender, message) {
  const messageElement = document.createElement('div');
  messageElement.classList.add('chat-message', sender);
  messageElement.textContent = message;
  chatMessages.appendChild(messageElement);
  chatMessages.scrollTop = chatMessages.scrollHeight;
}
