const chatContainer = document.getElementById('chat-container');
const chatToggleButton = document.getElementById('chat-toggle-button');
const chatMessages = document.getElementById('chat-messages');
const userInput = document.getElementById('user-input');
const sendButton = document.getElementById('send-button');

function generateSessionId() {
    return '_' + Math.random().toString(36).substr(2, 9);
}

let sessionId = localStorage.getItem('sessionId');
if (!sessionId) {
    sessionId = generateSessionId();
    localStorage.setItem('sessionId', sessionId);
}

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
          session_id: sessionId, 
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
      console.error('Error sending message:', error);
      addChatMessage('bot', 'An error has occurred. Please try again later.');
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
