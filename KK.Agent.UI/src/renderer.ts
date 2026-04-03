/**
 * This file will automatically be loaded by webpack and run in the "renderer" context.
 */

import './index.css';
import { marked } from 'marked';
import { v4 as uuidv4 } from 'uuid';

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
const messageInput = document.getElementById('messageInput') as HTMLTextAreaElement;
const sendBtn = document.getElementById('sendBtn') as HTMLButtonElement;
const statusDiv = document.getElementById('status') as HTMLElement;
const promptDropdown = document.getElementById('promptDropdown') as HTMLDivElement;

// Session management
let currentSessionId: string = uuidv4();

// Prompt autocomplete state
let currentPromptIndex = 0;
let availablePrompts: Array<{ name: string; filename: string }> = [];
let filteredPrompts: Array<{ name: string; filename: string }> = [];
let isInsertingPrompt = false;

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
    // Generate a random vibrant color for new agents (dark theme compatible)
    const colors = [
      'var(--accent-100)',
      'var(--accent-200)',
      'var(--accent-300)',
      'var(--accent-400)',
      'var(--accent-500)',
      'var(--accent-600)',
      'var(--accent-700)',
      'var(--accent-800)',
    ];
    agentColors[agentId] = colors[Math.floor(Math.random() * colors.length)];
  }
  return agentColors[agentId];
}

// Apply color styles to message element
function applyAgentColor(messageDiv: HTMLDivElement, agentId: string) {
  const color = getAgentColor(agentId);
  (messageDiv as HTMLElement).style.setProperty('--accent-color', color);
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

// Create a new session with fresh ID
function createNewSession() {
  currentSessionId = uuidv4();
  statusDiv.textContent = `Nowa sesja utworzona: ${currentSessionId}`;
  addNewMessage('System', `Utworzono nową sesję: ${currentSessionId}`, true);
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
        sessionId: currentSessionId,
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

function isPromptDropdownVisible(): boolean {
  return promptDropdown.style.display === 'block';
}

function hidePromptDropdown() {
  promptDropdown.style.display = 'none';
  promptDropdown.innerHTML = '';
  currentPromptIndex = 0;
  filteredPrompts = [];
}

async function ensurePromptsLoaded(): Promise<void> {
  if (!window.electronAPI) return;

  // Always try to refresh; prompts folder contents can change.
  const result = await window.electronAPI.readPromptsFolder();
  if (result.error || !result.files) {
    availablePrompts = [];
    return;
  }
  availablePrompts = result.files;
}

function getActiveAtToken(text: string, cursorPos: number): { start: number; end: number; query: string } | null {
  const left = text.slice(0, cursorPos);
  const atIndex = left.lastIndexOf('@');
  if (atIndex === -1) return null;

  // Only treat as token if it's at start or preceded by whitespace
  if (atIndex > 0 && !/\s/.test(left[atIndex - 1])) return null;

  const query = left.slice(atIndex + 1);
  // If there is whitespace inside the query, the token ended earlier
  if (/\s/.test(query)) return null;

  return { start: atIndex, end: cursorPos, query };
}

function applyFilter(query: string): Array<{ name: string; filename: string }> {
  const q = query.trim().toLowerCase();
  if (!q) return [...availablePrompts];
  return availablePrompts.filter(p => p.name.toLowerCase().includes(q) || p.filename.toLowerCase().includes(q));
}

function renderPromptDropdown() {
  // Render relative to input-container (which is position: relative)
  promptDropdown.style.display = 'block';
  promptDropdown.style.left = '0px';
  promptDropdown.style.top = `${messageInput.offsetHeight + 6}px`;

  if (filteredPrompts.length === 0) {
    promptDropdown.innerHTML = `
      <div class="prompt-dropdown-item" style="cursor: default; opacity: 0.8;">
        <span class="prompt-dropdown-name">Brak dopasowań</span>
      </div>
    `;
    return;
  }

  promptDropdown.innerHTML = filteredPrompts
    .map((prompt, index) => `
      <div class="prompt-dropdown-item ${index === currentPromptIndex ? 'active' : ''}" data-index="${index}">
        <span class="prompt-dropdown-icon">📄</span>
        <span class="prompt-dropdown-name">${prompt.name}</span>
        <span class="prompt-dropdown-hint">Tab/Enter</span>
      </div>
    `)
    .join('');

  const items = promptDropdown.querySelectorAll('.prompt-dropdown-item');
  items.forEach(item => {
    item.addEventListener('click', () => {
      const index = parseInt((item as HTMLElement).dataset.index || '0', 10);
      const prompt = filteredPrompts[index];
      if (prompt) void insertPrompt(prompt);
    });
  });
}

function updateDropdownHighlight() {
  const items = promptDropdown.querySelectorAll('.prompt-dropdown-item');
  items.forEach((item, index) => {
    if (index === currentPromptIndex) {
      (item as HTMLElement).classList.add('active');
      (item as HTMLElement).scrollIntoView({ block: 'nearest' });
    } else {
      (item as HTMLElement).classList.remove('active');
    }
  });
}

async function updatePromptDropdownFromInput() {
  const cursorPos = messageInput.selectionStart ?? messageInput.value.length;
  const token = getActiveAtToken(messageInput.value, cursorPos);

  if (!token) {
    if (isPromptDropdownVisible()) hidePromptDropdown();
    return;
  }

  // Token exists: `@<query>`
  if (!window.electronAPI) return;
  await ensurePromptsLoaded();

  filteredPrompts = applyFilter(token.query);
  currentPromptIndex = Math.min(currentPromptIndex, Math.max(filteredPrompts.length - 1, 0));

  renderPromptDropdown();
}

async function insertPrompt(prompt: { name: string; filename: string }) {
  if (!window.electronAPI) return;
  if (isInsertingPrompt) return;
  isInsertingPrompt = true;

  const cursorPos = messageInput.selectionStart ?? messageInput.value.length;
  const token = getActiveAtToken(messageInput.value, cursorPos);
  if (!token) {
    isInsertingPrompt = false;
    return;
  }

  try {
    const result = await window.electronAPI.readPromptFile(prompt.filename);
    if (result.error || result.content == null) {
      console.error('Error reading prompt:', result.error);
      return;
    }

    // Wstawiamy *tylko* zawartość pliku (bez nazwy i bez code fence'ów)
    const raw = result.content.replace(/\r\n/g, '\n');

    const before = messageInput.value.slice(0, token.start);
    const after = messageInput.value.slice(token.end);

    // Delikatne formatowanie: jeśli przed @ nie ma odstępu/nowej linii, dodajemy \n
    const needsLeadingNewline = before.length > 0 && !/[\s\n]$/.test(before);
    const insertion = (needsLeadingNewline ? '\n' : '') + raw;

    messageInput.value = before + insertion + after;

    const newCursorPos = (before + insertion).length;
    messageInput.setSelectionRange(newCursorPos, newCursorPos);

    hidePromptDropdown();
    messageInput.focus();
  } finally {
    isInsertingPrompt = false;
  }
}

document.addEventListener('DOMContentLoaded', () => {
  // Update session ID display on startup
  const sessionIdDisplay = document.getElementById('sessionIdDisplay') as HTMLElement;
  if (sessionIdDisplay) {
    sessionIdDisplay.textContent = `Sesja: ${currentSessionId}`;
  }

  // Create new session button
  const newSessionBtn = document.getElementById('newSessionBtn') as HTMLButtonElement;
  if (newSessionBtn) {
    newSessionBtn.addEventListener('click', () => {
      createNewSession();
      // Update display immediately
      if (sessionIdDisplay) {
        sessionIdDisplay.textContent = `Sesja: ${currentSessionId}`;
      }
    });
  }

  // Keep dropdown in sync with user typing (also enables filtering)
  messageInput.addEventListener('input', () => {
    void updatePromptDropdownFromInput();
  });

  // Keyboard handling: navigation + accept + send
  messageInput.addEventListener('keydown', (event) => {
    if (isPromptDropdownVisible()) {
      if (event.key === 'ArrowDown') {
        event.preventDefault();
        currentPromptIndex = Math.min(currentPromptIndex + 1, Math.max(filteredPrompts.length - 1, 0));
        updateDropdownHighlight();
        return;
      }
      if (event.key === 'ArrowUp') {
        event.preventDefault();
        currentPromptIndex = Math.max(currentPromptIndex - 1, 0);
        updateDropdownHighlight();
        return;
      }
      if (event.key === 'Tab' || event.key === 'Enter') {
        event.preventDefault();
        const prompt = filteredPrompts[currentPromptIndex];
        if (prompt) void insertPrompt(prompt);
        return;
      }
      if (event.key === 'Escape') {
        event.preventDefault();
        hidePromptDropdown();
        return;
      }
    }

    // Normal send (only when dropdown isn't open)
    // W textarea: Enter wysyła, Shift+Enter robi nową linię
    if (event.key === 'Enter' && !event.shiftKey) {
      event.preventDefault();
      void sendMessage();
    }
  });

  sendBtn.addEventListener('click', () => {
    void sendMessage();
  });

  // Hide dropdown when clicking outside
  document.addEventListener('click', (event) => {
    if (!promptDropdown.contains(event.target as Node) && event.target !== messageInput) {
      hidePromptDropdown();
    }
  });
});
