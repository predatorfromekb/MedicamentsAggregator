<template>
    <li class="search-result">{{title}}</li>
</template>

<script>
    export default {
        name: "SearchResult",
        props: {
            title: String
        }
    }
</script>

<style scoped>
    .search-result {
        padding: 10px 20px;
        cursor: pointer;
        border-bottom: 2px solid #ececec;
        border-left: 2px solid #ececec;
        border-right: 2px solid #ececec;
        display: block;
    }

    .search-result:hover {
        background-color: #eeeeee;
    }
</style>