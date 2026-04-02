// See the Electron documentation for details on how to use preload scripts:
// https://www.electronjs.org/docs/latest/tutorial/process-model#preload-scripts

import { contextBridge, ipcRenderer } from 'electron';

// Expose protected methods that allow the renderer process to use
// the ipcRenderer without exposing the entire object
contextBridge.exposeInMainWorld('electronAPI', {
  // Read list of files from prompts folder
  readPromptsFolder: () => ipcRenderer.invoke('read-prompts-folder'),
  // Read content of a specific prompt file
  readPromptFile: (filename: string) => ipcRenderer.invoke('read-prompt-file', filename),
});
