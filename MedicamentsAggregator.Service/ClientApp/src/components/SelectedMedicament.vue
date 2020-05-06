<template>
    <section class="selected-medicament">
        <span class="selected-medicament__title">{{medicament.title}}</span>
        <div class="selected-medicament__button-list">
            <button v-on:click="incrementCount" v-if="count < 9">➕</button>
            <button v-on:click="incrementCount" v-else disabled>➕</button>
            <button v-on:click="decrementCount" v-if="count > 1">➖</button>
            <button v-on:click="decrementCount" v-else disabled>➖</button>
            <span class="selected-medicament__count">{{count}}</span>
        </div>
    </section>
</template>

<script>
    export default {
        name: "SelectedMedicament",
        props: {
            medicament: Object
        },
        data : function () {
            return {
                count: this.medicament.count
            }
        },
        methods: {
            incrementCount : function () {
                this.count ++; /* Для локальной отрисовки */
                this.medicament.count ++; /* Для реального изменения количества */
            },
            decrementCount : function () {
                this.count --; /* Для локальной отрисовки */
                this.medicament.count --; /* Для реального изменения количества */
            }
        }
    }
</script>

<style scoped>
    .selected-medicament {
        display: grid;
        grid-template-columns: repeat(6, 1fr);
        
        white-space: nowrap;
        vertical-align: middle;
        
    }
    .selected-medicament__title {
        text-overflow: ellipsis;
        overflow: hidden;
        grid-column: span 5;
        padding-left: 5px;
    }

    .selected-medicament__count {
        text-align: center;
        border: 1px solid #cccccc;
        grid-column: span 1;
        width: 100%;
        -webkit-appearance: none;
        background-color: #eeeeee;
    }

    .selected-medicament__button-list {
        grid-column: span 1;
        display: grid;
        grid-template-columns: repeat(3, 1fr);
        grid-column-gap: 5px;
    }
    
</style>