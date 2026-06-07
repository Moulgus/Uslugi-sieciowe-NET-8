import { defineConfig } from 'vite'
import mkcert from 'vite-plugin-mkcert'

export default defineConfig({
  server: {
    https: true,
    port: 5174,
    strictPort: true
  },
  plugins: [mkcert()],
})
