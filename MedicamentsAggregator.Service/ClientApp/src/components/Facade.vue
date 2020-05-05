<template>
    <main class="facade">
        <section class="search-wrap">
            <Search />
        </section>
        <section class="selected-results">
            <div></div>
        </section>
    </main>
    
</template>

<script>
    import Search from "./Search";
    export default {
        name: "Facade",
        components: {Search}
    }
</script>

<style scoped>
    .facade {
        margin: auto; 
        width: 600px;
        height: 400px;
        display: grid;
        grid-template-rows: repeat(8, 1fr);
        grid-template-columns: repeat(12, 1fr);
    }
    
    .search-wrap {
        grid-row: 1/2;
        grid-column: 1/13;
    }
    
    .selected-results {
        grid-row: 3/8;
        grid-column: 1/13;
        background-color: #42b983;
    }
</style>