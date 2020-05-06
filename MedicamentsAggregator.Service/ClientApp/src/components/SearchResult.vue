<template>
    <li class="search-result" v-on:click="onResultClick">{{title}}</li>
</template>

<script>
    export default {
        name: "SearchResult",
        props: {
            title: String,
            url: String,
            id: String,
            commonData: Object,
            clearInput: Function
        },
        methods: {
            onResultClick: function () {
                const id = parseInt(this.id);
                if (!this.commonData.selectedMedicaments.has(id)){
                    this.commonData.selectedMedicaments.set(id, {
                        id: id,
                        count: 1,
                        title: this.title,
                        url: this.url,
                        
                    });
                    this.commonData.forceUpdate();
                    this.clearInput();
                }
            }
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