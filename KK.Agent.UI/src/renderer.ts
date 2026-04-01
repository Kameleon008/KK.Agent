/**
 * This file will automatically be loaded by webpack and run in the "renderer" context.
 */

import './index.css';

// DOM Elements
const messagesContainer = document.getElementById('messages') as HTMLElement;
const messageInput = document.getElementById('messageInput') as HTMLInputElement;
const sendBtn = document.getElementById('sendBtn') as HTMLButtonElement;
const statusDiv = document.getElementById('status') as HTMLElement;

// Add a message to the chat
function addMessage(agentId: string, text: string, isSystem: boolean = false) {
  const messageDiv = document.createElement('div');
  messageDiv.className = `message ${isSystem ? 'system' : 'agent'}`;
  
  if (isSystem) {
    messageDiv.textContent = text;
  } else {
    const agentSpan = document.createElement('span');
    agentSpan.style.color = '#4ecca3';
    agentSpan.style.fontWeight = 'bold';
    agentSpan.textContent = `[${agentId}] `;
    
    messageDiv.appendChild(agentSpan);
    messageDiv.textContent = text.replace(`[${agentId}] `, '');
  }
  
  messagesContainer.appendChild(messageDiv);
  messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Send message to the API endpoint
async function sendMessage() {
  const message = messageInput.value.trim();
  if (!message) return;
  
  // Clear input and disable button
  messageInput.value = '';
  sendBtn.disabled = true;
  statusDiv.textContent = 'Connecting...';
  
  try {
    addMessage('You', message, false);
    
    const response = await fetch('https://localhost:7084/chat/stream', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        sessionId: '',
        message: message,
      }),
    });
    
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    
    // Check if response is streamable
    const contentType = response.headers.get('content-type');
    
    if (contentType && contentType.includes('text/event-stream')) {
      // Handle Server-Sent Events (SSE)
      handleStream(response.body);
    } else {
      // Handle regular JSON response
      addMessage('System', 'Streaming not supported, got regular response', true);
    }
    
  } catch (error) {
    console.error('Error:', error);
    addMessage('Error', `Failed to send message: ${error.message}`, true);
    statusDiv.textContent = 'Ready';
    sendBtn.disabled = false;
  }
}

// Handle streaming response
async function handleStream(body: ReadableStream<Uint8Array>) {
  const reader = body.getReader();
  const decoder = new TextDecoder('utf-8');
  
  statusDiv.textContent = 'Receiving stream...';
  
  try {
    while (true) {
      const { done, value } = await reader.read();
      
      if (done) {
        break;
      }
      
      const chunk = decoder.decode(value, { stream: true });
      
      // Process each line in the chunk
      const lines = chunk.split('\n');
      
      for (const line of lines) {
        const trimmedLine = line.trim();
        
        if (!trimmedLine || !trimmedLine.startsWith('{')) {
          continue;
        }
        
        try {
          const data = JSON.parse(trimmedLine);
          
          if (data.agentId && data.message) {
            addMessage(data.agentId, data.message);
          } else {
            addMessage('System', `Received: ${JSON.stringify(data)}`, true);
          }
        } catch (e) {
          console.error('Failed to parse JSON:', trimmedLine);
        }
      }
    }
    
    statusDiv.textContent = 'Stream completed';
  } catch (error) {
    console.error('Stream error:', error);
    addMessage('Error', `Stream failed: ${error.message}`, true);
  } finally {
    reader.releaseLock();
    sendBtn.disabled = false;
  }
}

// Initialize - welcome message
addMessage('System', 'Welcome! Enter a message to start the conversation.', true);

document.addEventListener('DOMContentLoaded', () => {
  // Add Enter key handler
  messageInput.addEventListener('keypress', (event) => {
    if (event.key === 'Enter') {
      sendMessage();
    }
  });

  // Add click handler for send button
  sendBtn.addEventListener('click', () => {
    sendMessage();
  });
});
