import path from 'path'
import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'
import tailwindcss from '@tailwindcss/vite'
import { TanStackRouterVite } from '@tanstack/router-plugin/vite'
import svgr from 'vite-plugin-svgr'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    TanStackRouterVite({
      target: 'react',
      autoCodeSplitting: true,
    }),
    react(),
    tailwindcss(),
    svgr({ svgrOptions: { icon: true } }),
  ],
  server: {
    port: 6601,
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, './src'),
    },
  },
  build: {
    rollupOptions: {
      input: {
        '': path.resolve(__dirname, 'index.html'),
        'open/home': path.resolve(__dirname, 'open/home/index.html'),
        'open/chat': path.resolve(__dirname, 'open/chat/index.html'),
        'open/copilot': path.resolve(__dirname, 'open/copilot/index.html'),
        'ss-admin/apps': path.resolve(__dirname, 'ss-admin/apps/index.html'),
        'ss-admin/apps/flow': path.resolve(
          __dirname,
          'ss-admin/apps/flow/index.html'
        ),
        'ss-admin/apps/settings': path.resolve(
          __dirname,
          'ss-admin/apps/settings/index.html'
        ),
        'ss-admin/apps/messages': path.resolve(
          __dirname,
          'ss-admin/apps/messages/index.html'
        ),
        'ss-admin/apps/publish': path.resolve(
          __dirname,
          'ss-admin/apps/publish/index.html'
        ),
        'ss-admin/dataset/documents': path.resolve(
          __dirname,
          'ss-admin/dataset/documents/index.html'
        ),
        'ss-admin/dataset/settings': path.resolve(
          __dirname,
          'ss-admin/dataset/settings/index.html'
        ),
        'ss-admin/dataset/status': path.resolve(
          __dirname,
          'ss-admin/dataset/status/index.html'
        ),
        'ss-admin/dataset/testing': path.resolve(
          __dirname,
          'ss-admin/dataset/testing/index.html'
        ),
        'ss-admin/dataset/writer': path.resolve(
          __dirname,
          'ss-admin/dataset/writer/index.html'
        ),
        'ss-admin/settings/configsModels': path.resolve(
          __dirname,
          'ss-admin/settings/configsModels/index.html'
        ),
      },
    },
  },
})
