<template>
  <div id="app">
    <router-view></router-view>
  </div>
</template>

<script>
export default {
  name: 'App',
  components: {
  }
}
</script>

<style>
  #app {
    font-family: "Segoe UI",serif;
    display: flex;
    height: 100vh;
  }
  html, body {
    margin: 0;
  }
  input {
    font-family: "Segoe UI",serif;
    outline: none;
  }
  ul, button, div, span, input {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
  }
  button:not(:disabled) {
    cursor: pointer;
  }
</style>
