<template>
    <ul class="search-results">
        <SearchResult 
                v-for="result in results" 
                v-bind:key="result.DrugID" 
                :title="result.DrugName" 
        />
    </ul>
</template>

<script>
    import SearchResult from "./SearchResult";
    export default {
        name: "SearchResults",
        components: {SearchResult},
        props: ['results']
    }
</script>

<style scoped>
    .search-results {
        box-shadow: 0 15px 30px rgba(0,0,0,.1);
        
        /*Костыль для абсолютного позиционирования*/
        position: absolute;
        max-width: 600px;
        background-color: white;
    }
</style>