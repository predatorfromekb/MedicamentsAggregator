<template>
    <div>
        <div class="search-input-wrap">
            <input
                    placeholder="Поиск"
                    class="search"
                    autocomplete="false"
                    autofocus
                    v-model="query"
                    v-on:keyup="getSearchResults"
            />
        </div>
        <SearchResultList v-if="query"
                v-bind:results="results" 
                v-bind:commonData="commonData"
                v-bind:clearInput="clearInput.bind(this)" />
    </div>
</template>

<script>
    import SearchResultList from "./SearchResultList";
    
    export default {
        name: "Search",
        components: {SearchResultList},
        props: {
            commonData: Object
        },
        data: function () {
            return {
                query: '',
                results: [],
                cache: new Map()
            }
        },
        methods: {
            getSearchResults: function() {
                const query = this.query.toLowerCase();
                if (query.length < 5) {
                    this.results = [];
                    return;
                }
                if (this.cache.has(query)) {
                    this.results = this.cache.get(query);
                    return;
                }
                const formData = new FormData();
                formData.append('qs', query);
                fetch(`https://www.medgorodok.ru/ajax.php`, {
                    method: 'POST',
                    referrerPolicy: 'no-referrer',
                    body: formData
                })
                    .then(e => e.json())
                    .then(e => {
                        this.cache.set(query, e);
                        this.results = e;
                    });
            },
            clearInput: function () {
                this.query = '';
            }
        }
    }
</script>

<style scoped>
    .search-input-wrap {
        display: flex;
    }
    .search {
        padding-left: 20px;
        font-size: 20px;
        height: 50px;
        border: 2px solid #ececec;
        box-shadow: 0 0 30px rgba(0,0,0,.1);
        
        width: 100%;
        flex-grow: 1;
    }
</style>