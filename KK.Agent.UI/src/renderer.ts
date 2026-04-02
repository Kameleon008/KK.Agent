/**
 * This file will automatically be loaded by webpack and run in the "renderer" context.
 */

import './index.css';

// DOM Elements
const messagesContainer = document.getElementById('messages') as HTMLElement;
const messageInput = document.getElementById('messageInput') as HTMLInputElement;
const sendBtn = document.getElementById('sendBtn') as HTMLButtonElement;
const statusDiv = document.getElementById('status') as HTMLElement;

// Track all content sections for an agent message
interface MessageSection {
  type: 'content' | 'reasoning';
  span: HTMLSpanElement;
}

let currentAgentId: string | null = null;
let currentMessageDiv: HTMLDivElement | null = null;
let currentSections: MessageSection[] = [];

// Add a new message to the chat (for user/system messages)
function addNewMessage(agentId: string, text: string, isSystem = false) {
  const messageDiv = document.createElement('div');
  messageDiv.className = `message ${isSystem ? 'system' : 'agent'}`;
  
  if (isSystem) {
    messageDiv.textContent = text;
  } else {
    const agentSpan = document.createElement('span');
    agentSpan.style.color = '#4ecca3';
    agentSpan.style.fontWeight = 'bold';
    agentSpan.textContent = `[${agentId}] `;
    
    const contentSpan = document.createElement('span');
    contentSpan.textContent = text.replace(`[${agentId}] `, '');
    
    messageDiv.appendChild(agentSpan);
    messageDiv.appendChild(contentSpan);
  }
  
  messagesContainer.appendChild(messageDiv);
  messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Append text to current agent message or create new one
function appendToAgentMessage(agentId: string, text: string, isReasoning = false) {
  // If agent changed, create a new message bubble
  if (currentAgentId !== agentId) {
    // Save previous message if exists
    if (currentMessageDiv) {
      messagesContainer.appendChild(currentMessageDiv);
    }
    
    currentAgentId = agentId;
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message agent';
    
    const agentSpan = document.createElement('span');
    agentSpan.style.color = '#4ecca3';
    agentSpan.style.fontWeight = 'bold';
    agentSpan.textContent = `[${agentId}] `;
    
    messageDiv.appendChild(agentSpan);
    
    messagesContainer.appendChild(messageDiv);
    currentMessageDiv = messageDiv;
    currentSections = [];
  }
  
  // Find existing section of this type or create new one
  let section = currentSections.find(s => 
    (s.type === 'content' && !isReasoning) || 
    (s.type === 'reasoning' && isReasoning)
  );
  
  if (!section) {
    // Create new section
    const span = document.createElement('span');
    span.className = isReasoning ? 'reasoning' : 'content';
    span.textContent = ''; // Start empty for streaming
    
    currentMessageDiv.appendChild(span);
    
    section = { type: isReasoning ? 'reasoning' : 'content', span };
    currentSections.push(section);
  }
  
  // Append text to the appropriate span
  if (section) {
    section.span.textContent += text;
  }
  
  // Scroll to bottom
  messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Finalize and clear current agent message tracking
function finalizeAgentMessage() {
  if (currentMessageDiv) {
    messagesContainer.appendChild(currentMessageDiv);
    currentMessageDiv = null;
    currentSections = [];
  }
  currentAgentId = null;
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
    addNewMessage('You', message, false);
    
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
      addNewMessage('System', 'Streaming not supported, got regular response', true);
    }
    
  } catch (error) {
    console.error('Error:', error);
    addNewMessage('Error', `Failed to send message: ${error.message}`, true);
    statusDiv.textContent = 'Ready';
    sendBtn.disabled = false;
  }
}

// Handle streaming response
async function handleStream(body: ReadableStream<Uint8Array>) {
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
          
          if (data.agentId) {
            // Append content if present
            if (data.content) {
              appendToAgentMessage(data.agentId, data.content);
            }
            // Append reasoning if present
            else if (data.reasoning) {
              appendToAgentMessage(data.agentId, data.reasoning, true);
            }
          } else {
            addNewMessage('System', `Received: ${JSON.stringify(data)}`, true);
          }
        } catch (e) {
          console.error('Failed to parse JSON segment:', content);
        }
      }
    }
    
    finalizeAgentMessage();
    statusDiv.textContent = 'Stream completed';
  } catch (error) {
    console.error('Stream error:', error);
    addNewMessage('Error', `Stream failed: ${error.message}`, true);
  } finally {
    reader.releaseLock();
    sendBtn.disabled = false;
  }
}

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
