<template>
    <main class="facade">
        <section class="search-wrap">
            <Search v-bind:commonData="commonData"/>
        </section>
        
        <div class="controls">
            <div class="settings"></div>
            <button class="search-btn" v-on:click="sendMedicaments">Искать</button>
        </div>
        

        <div class="selected-medicament-list-wrap">
            <SelectedMedicament
                    v-for="[key,value] in commonData.selectedMedicaments"
                    v-bind:key="key"
                    :medicament="value"
            />
        </div>

        <div class="vld-parent">
            <VueLoading :active.sync="isLoading"
                     :can-cancel="false"
                     :is-full-page="true">
            </VueLoading>
        </div>
    </main>
    
</template>

<script>
    import VueLoading from 'vue-loading-overlay';
    import 'vue-loading-overlay/dist/vue-loading.css';
    import Search from "./Search";
    import SelectedMedicament from "./SelectedMedicament";
    export default {
        name: "Facade",
        components: {SelectedMedicament, Search, VueLoading},
        data: function () {
            return {
                isLoading : false,
                commonData: {
                    forceUpdate: this.$forceUpdate.bind(this),
                    selectedMedicaments: new Map()
                }
            }
        },
        methods: {
            sendMedicaments: function () {
                const medicaments = Array.from(this.commonData.selectedMedicaments.values());
                console.log(medicaments);
                this.isLoading = true;
                // simulate AJAX
                setTimeout(() => {
                    this.isLoading = false
                },5000)
            }
        }
    }
    
</script>

<style scoped>
    .facade {
        margin: auto; 
        width: 600px;
        height: 400px;
        display: grid;
        grid-template-rows: 50px 50px 300px;
        grid-template-columns: 1fr;
        grid-row-gap: 5px;
    }
    
    .search-wrap {
        grid-row: 1/2;
    }
    
    .controls {
        grid-row: 2/3;
        display: grid;
        grid-template-columns: repeat(6, 1fr);
    }
    .settings {
        grid-column: 1/6;
    }
    .search-btn {
        grid-column: 6/7;
        width: 100%;
        height: 100%;
    }
    
    .selected-medicament-list-wrap {
        display: grid;
        grid-template-rows: repeat(12, 1fr);
        grid-row-gap: 5px;
    }
</style>