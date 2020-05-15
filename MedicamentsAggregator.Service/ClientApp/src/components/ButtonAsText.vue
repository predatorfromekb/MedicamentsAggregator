<template>
    <button class="button-as-text" disabled>{{text}}</button>
</template>

<script>
    export default {
        name: "ButtonAsText",
        props: {
            text: String
        }
    }
</script>

<style scoped>
    .button-as-text {
        border: none;
        background-color: transparent;
        color: black;
    }
</style>