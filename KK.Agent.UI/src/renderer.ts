/**
 * This file will automatically be loaded by webpack and run in the "renderer" context.
 */

import './index.css';
import { marked } from 'marked';

// Configure marked for safety and styling (use sync mode for streaming)
marked.setOptions({
  breaks: true,
  gfm: true,
  async: false, // Force synchronous parsing for live rendering
});

// Helper function to parse markdown synchronously
const parseMarkdown = (text: string): string => {
  return marked.parse(text, { async: false }) as string;
};

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

// Map of AgentId to accent colors (default to yellow/gold)
const agentColors: Record<string, string> = {};

let currentAgentId: string | null = null;
let currentMessageDiv: HTMLDivElement | null = null;
let currentSections: MessageSection[] = [];

// Get or generate color for an agent
function getAgentColor(agentId: string): string {
  if (!agentColors[agentId]) {
    // Generate a random vibrant color for new agents
    const colors = [
      '#ffd700', // gold
      '#ff6b6b', // red
      '#4ecdc4', // teal
      '#95e1d3', // mint
      '#f38181', // pink
      '#aa96da', // purple
      '#fcbad3', // light pink
      '#a8e6cf', // green
      '#ff8b94', // coral
      '#74b9ff', // blue
    ];
    agentColors[agentId] = colors[Math.floor(Math.random() * colors.length)];
  }
  return agentColors[agentId];
}

// Apply color styles to message element
function applyAgentColor(messageDiv: HTMLDivElement, agentId: string) {
  const color = getAgentColor(agentId);
  (messageDiv as HTMLElement).style.setProperty('--accent-color', color);
  
  // Also set inline style for header (fallback if CSS variable not supported)
  const headerSpan = messageDiv.querySelector('.header');
  if (headerSpan) {
    (headerSpan as HTMLElement).style.color = color;
  }
}

// Add a new message to the chat (for user/system messages)
function addNewMessage(agentId: string, text: string, isSystem = false) {
  const messageDiv = document.createElement('div');
  messageDiv.className = `message ${isSystem ? 'system' : 'user'}`;
  
  if (isSystem) {
    messageDiv.textContent = text;
  } else {
    // Header span with name
    const headerSpan = document.createElement('span');
    headerSpan.className = 'header';
    headerSpan.textContent = `[${agentId}]`;
    
    // Content span - only create if content exists after removing agent prefix
    const contentText = text.replace(`[${agentId}] `, '');
    if (contentText) {
      const contentSpan = document.createElement('span');
      contentSpan.className = 'content';
      contentSpan.innerHTML = parseMarkdown(contentText);
      
      messageDiv.appendChild(headerSpan);
      messageDiv.appendChild(contentSpan);
    } else {
      messageDiv.appendChild(headerSpan);
    }
  }
  
  messagesContainer.appendChild(messageDiv);
  messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Store pending markdown content for agent messages
interface PendingContent {
  type: 'content' | 'reasoning';
  span: HTMLSpanElement;
  rawText: string;
}

let pendingContents: PendingContent[] = [];

// Append text to current agent message or create new one
function appendToAgentMessage(agentId: string, text: string, isReasoning = false) {
  // If agent changed, create a new message bubble
  if (currentAgentId !== agentId) {
    // Save previous message if exists
    if (currentMessageDiv) {
      messagesContainer.appendChild(currentMessageDiv);
      currentMessageDiv = null;
    }
    
    currentAgentId = agentId;
    const messageDiv = document.createElement('div');
    messageDiv.className = 'message agent';
    
    // Header span with name
    const headerSpan = document.createElement('span');
    headerSpan.className = 'header';
    headerSpan.textContent = `[${agentId}]`;
    
    messageDiv.appendChild(headerSpan);
    
    messagesContainer.appendChild(messageDiv);
    applyAgentColor(messageDiv, agentId);
    currentMessageDiv = messageDiv;
    currentSections = [];
    pendingContents = [];
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
    
    currentMessageDiv.appendChild(span);
    
    section = { type: isReasoning ? 'reasoning' : 'content', span };
    currentSections.push(section);
    
    // Track pending content for markdown rendering
    const rawText = text || '';
    pendingContents.push({ type: isReasoning ? 'reasoning' : 'content', span, rawText });
  } else {
    // Append text to existing section's raw text (not HTML yet)
    const pendingContent = pendingContents.find(pc => pc.type === section!.type);
    if (pendingContent) {
      pendingContent.rawText += text;
    }
  }
  
  // Render markdown live for the current section type
  const pendingContent = pendingContents.find(pc => pc.type === section!.type);
  if (pendingContent) {
    section.span.innerHTML = parseMarkdown(pendingContent.rawText);
  }
  
  // Scroll to bottom
  messagesContainer.scrollTop = messagesContainer.scrollHeight;
}

// Render markdown for all pending content in a message (for finalization)
async function renderPendingMarkdown() {
  // When async is disabled, parse returns string directly, but TS still thinks it's Promise
  const htmls = await Promise.all(pendingContents.map(pending => marked.parse(pending.rawText)));
  
  for (let i = 0; i < pendingContents.length; i++) {
    pendingContents[i].span.innerHTML = htmls[i] as string;
  }
  pendingContents = [];
}

// Finalize and clear current agent message tracking
async function finalizeAgentMessage() {
  if (currentMessageDiv) {
    // Render markdown for all pending content before finalizing
    await renderPendingMarkdown();
    
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
