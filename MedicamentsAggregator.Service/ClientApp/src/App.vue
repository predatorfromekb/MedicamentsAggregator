<template>
  <div id="app">
    <Facade />
  </div>
</template>

<script>
import Facade from "./components/Facade";

export default {
  name: 'App',
  components: {
    Facade,
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
  input[type=number] {
    -moz-appearance: textfield;
  }
  input::-webkit-inner-spin-button {
    -webkit-appearance: none;
    margin: 0;
  }
</style>
