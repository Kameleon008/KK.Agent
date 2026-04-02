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
function addMessage(agentId: string, text: string, isSystem = false) {
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
async function handleStream(body: ReadableStream<Uint8Array<ArrayBuffer>>) {
  const reader = body.getReader();
  const decoder = new TextDecoder('utf-8');
  let buffer = ''; // Accumulate partial data here
  
  statusDiv.textContent = 'Receiving stream...';
  
  try {
    let done = false;
    while (!done) {
      const result = await reader.read();
      done = result.done;
      const value = result.value;
      if (done) break;
      
      // Append new data to the buffer
      buffer += decoder.decode(value, { stream: true });
      
      // Process the buffer line by line
      const lines = buffer.split('\n');
      
      // Keep the last (potentially incomplete) line in the buffer
      buffer = lines.pop() || ''; 
      
      for (const line of lines) {
        const trimmedLine = line.trim();
        
        // SSE standard: lines usually start with "data: "
        // If your server just sends raw JSON strings per line, remove the .replace
        let content = trimmedLine;
        if (content.startsWith('data: ')) {
          content = content.replace('data: ', '');
        }

        if (!content || content === '[DONE]') continue; 

        try {
          const data = JSON.parse(content);
          
          if (data.agentId && data.message) {
            // TIP: You might want to append text to the same message bubble 
            // if it's a "token" stream, rather than creating a new bubble for every chunk.
            addMessage(data.agentId, data.message);
          } else {
            addMessage('System', `Received: ${JSON.stringify(data)}`, true);
          }
        } catch (e) {
          console.error('Failed to parse JSON segment:', content);
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
