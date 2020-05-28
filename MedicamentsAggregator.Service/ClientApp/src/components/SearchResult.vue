<template>
    <li class="search-result" v-on:click="onResultClick">
        <div class="search-result__title">{{title}}</div>
<!--        <div class="search-result__pharmacies-count">{{pharmaciesCount}} {{wordEndingByNumber(pharmaciesCount, 'аптека', 'апетеки','аптек')}}</div>-->
    </li>
</template>

<script>
    export default {
        name: "SearchResult",
        props: {
            title: String,
            url: String,
            id: String,
            pharmaciesCount: String,
            forceUpdate: Function,
            selectedMedicaments: Map,
            clearInput: Function
        },
        methods: {
            onResultClick: function () {
                const id = parseInt(this.id);
                if (!this.selectedMedicaments.has(id)){
                    this.selectedMedicaments.set(id, {
                        id: id,
                        count: 1,
                        title: this.title,
                        url: this.url,
                        
                    });
                    this.forceUpdate();
                    this.clearInput();
                }
            },
            wordEndingByNumber: function (number, one, two, five)
            {
                number = number % 100;
                if (number > 20) number %= 10;
                if (number === 1) return one;
                if (number > 1 && number < 5) return two;
                return five;
            }

    }
    }
</script>

<style scoped>
    .search-result {
        
        cursor: pointer;
        border-bottom: 2px solid #ececec;
        border-left: 2px solid #ececec;
        border-right: 2px solid #ececec;
        display: grid;
    }
    
    .search-result__title {
        padding: 10px 20px;
    }
    
    .search-result__pharmacies-count {
        padding: 10px 10px;
        background-color: #f7f7f7;
    }

    .search-result:hover {
        background-color: #eeeeee;
    }

    .search-result:hover .search-result__pharmacies-count{
        background-color: #e7e7e7;
    }
</style>