<template>
    <ul class="search-results">
        <SearchResult 
                v-for="{DrugID, DrugName, url} in results
                    .filter(e => !commonData.selectedMedicaments.has(parseInt(e.DrugID)))" 
                v-bind:key="DrugID" 
                :title="DrugName" 
                :url="url"
                :id="DrugID"
                :commonData="commonData"
                v-bind:clearInput="clearInput"
        />
    </ul>
</template>

<script>
    import SearchResult from "./SearchResult";
    export default {
        name: "SearchResultList",
        components: {SearchResult},
        props: {
            results: Array,
            commonData: Object,
            clearInput: Function
        }
    }
</script>

<style scoped>
    .search-results {
        box-shadow: 0 15px 30px rgba(0,0,0,.1);
        
        /*Костыль для абсолютного позиционирования*/
        position: absolute;
        width: 600px;
        background-color: white;
        z-index: 15001;
    }
    @media screen and (max-width: 600px) {
        .search-results {
            width: 100%;
        }
    }
    
</style>